using Newtonsoft.Json;
using Whisper.Data;
using Whisper.Extensions;
using Whisper.Services.Serializers.Infrastructure;

namespace Whisper.Services.Serializers;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SignalRDataJsonSerializer(JsonSerializerSettings settings) : JsonSerializer<SignalRData>(settings)
{
    private readonly JsonSerializerSettings _settings = settings;

    public override string Serialize(SignalRData @object)
    {
        return _settings.Serialize(@object.Data);
    }

    public override SignalRData Deserialize(string json)
    {
        return new SignalRData(_settings.Deserialize<string>(json));
    }
}