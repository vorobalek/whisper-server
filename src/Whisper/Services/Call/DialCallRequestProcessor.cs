using Microsoft.AspNetCore.SignalR;
using Whisper.Hubs;
using Whisper.Models.Calls.Dial;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Call.Infrastructure;
using Whisper.Services.Notifications;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage;

namespace Whisper.Services.Call;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DialCallRequestProcessor(
    IJsonSerializer<ICallData> callDataSerializer,
    ISignalRDataStorage signalRDataStorage,
    ISubscriptionStorage subscriptionStorage,
    INotificationProvider notificationProvider,
    IHubContext<SignalV1Hub> signalHubContext) :
    TransmittableCallRequestProcessor<DialCallRequest, DialCallData>(
        callDataSerializer,
        signalRDataStorage,
        subscriptionStorage,
        notificationProvider,
        signalHubContext)
{
    protected override INotification CreateNotification(Dictionary<string, string> data)
    {
        return new DialNotification(data);
    }

    private record DialNotification(Dictionary<string, string> Data) : INotification
    {
        public string Title => "🛰️ Hey! Are you here?";
        public string Body => "Someone is trying to reach you!";
    }
}