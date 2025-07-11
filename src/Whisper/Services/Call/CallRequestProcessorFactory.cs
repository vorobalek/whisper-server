using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Call.Infrastructure;

namespace Whisper.Services.Call;

internal sealed class CallRequestProcessorFactory(
    IEnumerable<ICallRequestProcessor> callRequestProcessors) : ICallRequestProcessorFactory
{
    public ICallRequestProcessor GetForRequest(ICallRequest request)
    {
        return callRequestProcessors.Single(x => x.CallRequestType == request.GetType());
    }
}