using JsonSubTypes;
using Newtonsoft.Json;
using Whisper.Models.Calls.Answer;
using Whisper.Models.Calls.Close;
using Whisper.Models.Calls.Dial;
using Whisper.Models.Calls.Ice;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Models.Calls.Offer;
using Whisper.Models.Calls.Update;

namespace Whisper.Services.Serializers.Infrastructure;

internal static class JsonConverters
{
    private const string CallRequestDiscriminatorPropertyName = "a";

    public static JsonConverter CallRequest =>
        JsonSubtypesConverterBuilder
            .Of<ICallRequest>(CallRequestDiscriminatorPropertyName)
            .SetFallbackSubtype<CallRequestBase>()
            .RegisterSubtype<UpdateCallRequest>(UpdateCallRequest.MethodName)
            .RegisterSubtype<DialCallRequest>(DialCallRequest.MethodName)
            .RegisterSubtype<OfferCallRequest>(OfferCallRequest.MethodName)
            .RegisterSubtype<AnswerCallRequest>(AnswerCallRequest.MethodName)
            .RegisterSubtype<IceCallRequest>(IceCallRequest.MethodName)
            .RegisterSubtype<CloseCallRequest>(CloseCallRequest.MethodName)
            .Build();
}