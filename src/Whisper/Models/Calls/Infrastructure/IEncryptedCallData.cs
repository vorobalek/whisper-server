using Newtonsoft.Json;

namespace Whisper.Models.Calls.Infrastructure;

public interface IEncryptedCallData : IEncryptionHolderCallData
{
    [JsonProperty("e", Required = Required.Always, Order = 4)]
    public string EncryptedDataBase64 { get; init; }
}