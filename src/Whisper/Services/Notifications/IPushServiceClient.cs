using Lib.Net.Http.WebPush;

namespace Whisper.Services.Notifications;

public interface IPushServiceClient
{
    Task RequestPushMessageDeliveryAsync(
        PushSubscription subscription,
        PushMessage message,
        CancellationToken cancellationToken);
}