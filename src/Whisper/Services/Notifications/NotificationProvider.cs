using Lib.Net.Http.WebPush;
using Whisper.Services.Serializers.Infrastructure;

namespace Whisper.Services.Notifications;

internal sealed class NotificationProvider(
    IJsonSerializer<INotification> notificationSerializer,
    IPushServiceClient pushServiceClient) :
    INotificationProvider
{
    public async Task TrySendAsync(
        ISubscription subscription,
        INotification notification,
        CancellationToken cancellationToken)
    {
        var content = notificationSerializer.Serialize(notification);
        var pushSubscription = new PushSubscription
        {
            Endpoint = subscription.Endpoint,
            Keys = new Dictionary<string, string>
            {
                ["p256dh"] = subscription.GetKeys().P256Dh,
                ["auth"] = subscription.GetKeys().Auth
            }
        };
        var pushMessage = new PushMessage(content)
        {
            Urgency = PushMessageUrgency.High
        };
        await pushServiceClient.RequestPushMessageDeliveryAsync(
            pushSubscription,
            pushMessage,
            cancellationToken);
    }
}