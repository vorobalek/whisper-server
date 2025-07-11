using Microsoft.AspNetCore.SignalR;
using Whisper.Data;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Call.Infrastructure;
using Whisper.Services.GlobalCancellationToken;
using Whisper.Storage;

namespace Whisper.Hubs;

public class SignalV1Hub(
    ISignalRDataStorage signalRDataStorage,
    ICallProcessor callProcessor,
    ILogger<SignalV1Hub> logger,
    IGlobalCancellationTokenSource cancellationTokenSource) : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await signalRDataStorage
            .DeleteAsync(
                Context.ConnectionId,
                cancellationTokenSource.Token);
        await base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("call")]
    public async Task<ICallResponse?> CallAsync(ICallRequest request)
    {
        ICallResponse? result;
        try
        {
            var cancellationToken = cancellationTokenSource.Token;
            result = await callProcessor.ProcessAsync(request, cancellationToken);
            await UpsertSignalRDataAsync(request.GetData().PublicKey, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Internal server error");
            throw new ApplicationException("Internal server error");
        }

        return result;
    }

    private Task<bool> UpsertSignalRDataAsync(
        string publicKey,
        CancellationToken cancellationToken)
    {
        return signalRDataStorage.UpsertAsync(
            publicKey,
            new SignalRData(Context.ConnectionId),
            cancellationToken,
            TimeSpan.FromMinutes(2));
    }
}