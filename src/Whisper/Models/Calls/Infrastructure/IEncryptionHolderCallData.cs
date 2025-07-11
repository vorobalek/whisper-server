using Newtonsoft.Json;

namespace Whisper.Models.Calls.Infrastructure;

public interface IEncryptionHolderCallData : ITransmittableCallData
{
    [JsonProperty("d", Required = Required.Always, Order = 3)]
    public string PublicEncryptionKey { get; init; }
}