using Whisper.Models.Calls.Infrastructure;
using Whisper.Models.Calls.Update;
using Whisper.Services.Call.Infrastructure;
using Whisper.Storage;

namespace Whisper.Services.Call;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class UpdateCallRequestProcessor(
    ISubscriptionStorage subscriptionDataStorage) :
    CallRequestProcessor<UpdateCallRequest>
{
    public override async Task<ICallResponse?> ProcessAsync(UpdateCallRequest request, CancellationToken cancellationToken)
    {
        if (request.Data.Subscription is not null)
        {
            return await subscriptionDataStorage.UpsertAsync(
                request.Data.PublicKey,
                request.Data.Subscription,
                cancellationToken,
                TimeSpan.FromDays(180))
                ? CreateSuccessResponse(request)
                : CreateErrorResponse(request, $"Unable to update subscription for {request.Data.PublicKey}.");
        }

        return CreateSuccessResponse(request);
    }
}