using Newtonsoft.Json;
using Whisper.Extensions;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Serializers.Infrastructure;

namespace Whisper.Services.Serializers;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CallRequestJsonSerializer : JsonSerializer<ICallRequest>
{
    public CallRequestJsonSerializer(JsonSerializerSettings settings) : base(settings)
    {
        settings.AddConverters([JsonConverters.CallRequest]);
    }
}