using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Offer;

internal sealed record OfferCallRequest : CallRequest<OfferCallData>
{
    public const string MethodName = "offer";
}