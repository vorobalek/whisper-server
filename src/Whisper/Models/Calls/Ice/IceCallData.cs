using Newtonsoft.Json;
using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Ice;

internal sealed record IceCallData : EncryptedCallData
{
    [JsonProperty("f", Required = Required.Always, Order = 5)]
    public IceDirection Direction { get; init; } = IceDirection.Unknown;
}