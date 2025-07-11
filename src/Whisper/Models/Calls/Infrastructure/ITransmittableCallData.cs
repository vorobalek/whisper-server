using Newtonsoft.Json;

namespace Whisper.Models.Calls.Infrastructure;

public interface ITransmittableCallData : ICallData
{
    [JsonProperty("b", Required = Required.Always, Order = 1)]
    public long TimeStamp { get; init; }

    [JsonProperty("c", Required = Required.Always, Order = 2)]
    public string PeerPublicKey { get; init; }
}