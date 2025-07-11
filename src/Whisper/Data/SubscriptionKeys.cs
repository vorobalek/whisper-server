using Newtonsoft.Json;
using Whisper.Services.Notifications;

namespace Whisper.Data;

public record SubscriptionKeys : ISubscriptionKeys
{
    [JsonProperty("a", Required = Required.Always, Order = 0)]
    public string P256Dh { get; init; } = null!;

    [JsonProperty("b", Required = Required.Always, Order = 1)]
    public string Auth { get; init; } = null!;
}