using Microsoft.AspNetCore.SignalR;
using Whisper.Hubs;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Notifications;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage;

namespace Whisper.Services.Call.Infrastructure;

internal abstract class TransmittableCallRequestProcessor<TRequest, TData>(
    IJsonSerializer<ICallData> callDataSerializer,
    ISignalRDataStorage signalRDataStorage,
    ISubscriptionStorage subscriptionStorage,
    INotificationProvider notificationProvider,
    IHubContext<SignalV1Hub> signalHubContext) :
    CallRequestProcessor<TRequest>
    where TRequest : ICallRequest<TData>
    where TData : ITransmittableCallData
{
    public override async Task<ICallResponse?> ProcessAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var requestData = callDataSerializer.Serialize(request.Data);
        var data = new Dictionary<string, string>
        {
            ["a"] = request.Method,
            ["b"] = requestData,
            ["c"] = request.Signature
        };

        var signalRData = await signalRDataStorage
            .ReadAsync(
                request.Data.PeerPublicKey,
                cancellationToken);

        if (signalRData is not null)
        {
            await SendSignalAsync(signalRData.Data, data, cancellationToken);
            return CreateSuccessResponse(request);
        }

        var subscription = await subscriptionStorage
            .ReadAsync(
                request.Data.PeerPublicKey,
                cancellationToken);

        if (subscription is null)
            return CreateErrorResponse(request, $"Account {request.Data.PeerPublicKey} is not reachable.");

        var notification = CreateNotification(data);
        await notificationProvider.TrySendAsync(
            subscription,
            notification,
            cancellationToken);

        return CreateSuccessResponse(request);
    }

    protected abstract INotification CreateNotification(Dictionary<string, string> data);

    private async Task SendSignalAsync(
        string connectionId,
        Dictionary<string, string> data,
        CancellationToken cancellationToken)
    {
        await signalHubContext
            .Clients
            .Client(connectionId)
            .SendAsync(
                "call",
                data,
                cancellationToken);
    }
}