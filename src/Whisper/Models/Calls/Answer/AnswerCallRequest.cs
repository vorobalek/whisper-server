using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Models.Calls.Answer;

internal sealed record AnswerCallRequest : CallRequest<AnswerCallData>
{
    public const string MethodName = "answer";
}