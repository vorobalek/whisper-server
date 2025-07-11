using Whisper.Models.Calls.Infrastructure;

namespace Whisper.Services.Call.Infrastructure;

public interface ICallProcessor
{
    Task<ICallResponse?> ProcessAsync(ICallRequest request, CancellationToken cancellationToken);
}