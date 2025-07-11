using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Update;

internal sealed record UpdateCallRequest : CallRequest<UpdateCallData>
{
    public const string MethodName = "update";
}