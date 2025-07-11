namespace Whisper.Services.Notifications;

public interface INotificationProvider
{
    public Task TrySendAsync(
        ISubscription subscription,
        INotification notification,
        CancellationToken cancellationToken);
}