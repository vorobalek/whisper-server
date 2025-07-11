namespace Whisper.Services.GlobalCancellationToken;

public interface IGlobalCancellationTokenSource
{
    CancellationToken Token { get; }
}