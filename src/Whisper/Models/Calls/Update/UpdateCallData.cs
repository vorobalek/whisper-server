using Newtonsoft.Json;
using Whisper.Data;
using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Update;

internal sealed record UpdateCallData : CallData
{
    [JsonProperty("b", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
    public Subscription? Subscription { get; init; }
}