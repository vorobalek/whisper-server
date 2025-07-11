using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Ice;

internal sealed record IceCallRequest : CallRequest<IceCallData>
{
    public const string MethodName = "ice";
}