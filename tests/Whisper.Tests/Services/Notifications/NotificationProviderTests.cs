using Lib.Net.Http.WebPush;
using Moq;
using Whisper.Data;
using Whisper.Services.Notifications;
using Whisper.Services.Serializers.Infrastructure;

namespace Whisper.Tests.Services.Notifications;

[TestClass]
public class NotificationProviderTests
{
    [TestMethod]
    public async Task TrySendAsync_Should_Send_Push_Notification()
    {
        // Arrange
        var title = Unique.String();
        var body = Unique.String();
        var data = new Dictionary<string, string>
        {
            [Unique.String()] = Unique.String(),
            [Unique.String()] = Unique.String()
        };
        var notification = new Notification(title, body, data);
        var endpoint = Unique.Url();
        var p256Dh = Unique.String();
        var auth = Unique.String();
        var subscription = new Subscription
        {
            Endpoint = endpoint,
            Keys = new SubscriptionKeys
            {
                P256Dh = p256Dh,
                Auth = auth
            }
        };
        var cancellationToken = CancellationToken.None;
        var content = Unique.String();

        var jsonSerializerMock = new Mock<IJsonSerializer<INotification>>(MockBehavior.Strict);
        jsonSerializerMock
            .Setup(serializer => serializer
                .Serialize(It.Is<INotification>(x =>
                    x.Title == title &&
                    x.Body == body &&
                    x.Data == data)))
            .Returns(content)
            .Verifiable(Times.Once);

        var pushServiceClientMock = new Mock<IPushServiceClient>(MockBehavior.Strict);
        pushServiceClientMock
            .Setup(pushServiceClient => pushServiceClient
                .RequestPushMessageDeliveryAsync(
                    It.Is<PushSubscription>(pushSubscription =>
                        pushSubscription.Endpoint == endpoint &&
                        pushSubscription.Keys["p256dh"] == p256Dh &&
                        pushSubscription.Keys["auth"] == auth),
                    It.Is<PushMessage>(pushMessage =>
                        pushMessage.Content == content &&
                        pushMessage.Urgency == PushMessageUrgency.High),
                    cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        var notificationProvider = new NotificationProvider(
            jsonSerializerMock.Object,
            pushServiceClientMock.Object);

        // Act
        await notificationProvider.TrySendAsync(
            subscription,
            notification,
            cancellationToken);

        // Assert
        jsonSerializerMock.VerifyAll();
        jsonSerializerMock.VerifyNoOtherCalls();
        pushServiceClientMock.VerifyAll();
        pushServiceClientMock.VerifyNoOtherCalls();
    }

    private record Notification(string Title,
        string Body,
        Dictionary<string, string> Data) : INotification;
}