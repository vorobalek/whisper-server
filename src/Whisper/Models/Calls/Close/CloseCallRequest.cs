using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Close;

internal sealed record CloseCallRequest : CallRequest<CloseCallData>
{
    public const string MethodName = "close";
}