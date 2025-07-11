using Lib.Net.Http.WebPush;

namespace Whisper.Services.Notifications;

internal sealed class DefaultPushServiceClient(PushServiceClient client) : IPushServiceClient
{
    public Task RequestPushMessageDeliveryAsync(
        PushSubscription subscription,
        PushMessage message,
        CancellationToken cancellationToken)
    {
        return client.RequestPushMessageDeliveryAsync(subscription, message, cancellationToken);
    }
}