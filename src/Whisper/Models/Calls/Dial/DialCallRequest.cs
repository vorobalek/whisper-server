using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Dial;

internal sealed record DialCallRequest : CallRequest<DialCallData>
{
    public const string MethodName = "dial";
}