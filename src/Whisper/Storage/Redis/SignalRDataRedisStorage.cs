using StackExchange.Redis;
using Whisper.Data;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage.Redis.Infrastructure;

namespace Whisper.Storage.Redis;

internal sealed class SignalRDataRedisStorage(
    IConnectionMultiplexer connectionMultiplexer,
    IJsonSerializer<SignalRData> serializer) :
    RedisStorage<SignalRData>(connectionMultiplexer, serializer),
    ISignalRDataStorage
{
    protected override int DatabaseId => RedisDatabaseIds.SignalData;
    protected override string KeyPrefix => "signalR";

    protected override async Task<bool> UpsertInternalAsync(
        string key,
        SignalRData value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        return await base.UpsertInternalAsync(key, value, cancellationToken, expiry)
               && await base.UpsertInternalAsync(value.Data, new SignalRData(key), cancellationToken, expiry);
    }

    protected override async Task<bool> DeleteInternalAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var data = await ReadAsync(key, cancellationToken);
        return await base.DeleteInternalAsync(key, cancellationToken)
               && (data is null || await base.DeleteInternalAsync(data.Data, cancellationToken));
    }
}