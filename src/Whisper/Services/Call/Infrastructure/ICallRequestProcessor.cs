using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Services.Call.Infrastructure;

public interface ICallRequestProcessor
{
    Type CallRequestType { get; }
    Task<ICallResponse?> ProcessAsync(ICallRequest request, CancellationToken cancellationToken);
}

public interface ICallRequestProcessor<in TCallRequest> : ICallRequestProcessor
    where TCallRequest : ICallRequest
{
    Task<ICallResponse?> ProcessAsync(TCallRequest request, CancellationToken cancellationToken);
}