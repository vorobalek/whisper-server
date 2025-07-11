namespace Whisper.Services.DbDateTime;

public interface IDbDateTimeProvider
{
    public Task<long> GetMongoUnixTimeMillisecondsAsync(CancellationToken cancellationToken);
}