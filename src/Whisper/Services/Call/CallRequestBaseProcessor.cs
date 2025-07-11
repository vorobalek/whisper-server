using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Call.Infrastructure;

namespace Whisper.Services.Call;

internal sealed class CallRequestBaseProcessor : CallRequestProcessor<CallRequestBase>
{
    public override Task<ICallResponse?> ProcessAsync(CallRequestBase request, CancellationToken cancellationToken)
    {
        return Task.FromResult<ICallResponse?>(null);
    }
}