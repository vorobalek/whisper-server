using MongoDB.Bson;
using MongoDB.Driver;

namespace Whisper.Services.DbDateTime;

internal sealed class DbDateTimeProvider(IMongoDatabase database) : IDbDateTimeProvider
{
    public async Task<long> GetMongoUnixTimeMillisecondsAsync(CancellationToken cancellationToken)
    {
        var command = new BsonDocument("hostInfo", 1);

        var response = await database.RunCommandAsync<BsonDocument>(
            command,
            ReadPreference.Primary,
            cancellationToken
        );

        var currentTimeBson = response["system"]["currentTime"].AsBsonDateTime;
        var serverDateTimeUtc = currentTimeBson.ToUniversalTime();

        return new DateTimeOffset(serverDateTimeUtc).ToUnixTimeMilliseconds();
    }
}