using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Services.Call.Infrastructure;

public interface ICallRequestProcessorFactory
{
    ICallRequestProcessor GetForRequest(ICallRequest request);
}