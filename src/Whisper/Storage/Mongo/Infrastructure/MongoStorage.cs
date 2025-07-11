using MongoDB.Driver;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage.Infrastructure;

namespace Whisper.Storage.Mongo.Infrastructure;

internal abstract class MongoStorage<TValue> : IStorage<string, TValue>
{
    private readonly IMongoCollection<MongoDocument> _collection;
    private readonly Func<TValue, DateTime?>? _expirationGetter;
    private readonly IJsonSerializer<TValue> _serializer;

    protected MongoStorage(
        IMongoDatabase database,
        IJsonSerializer<TValue> serializer,
        Func<TValue, DateTime?>? expirationGetter = null)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        _collection = database.GetCollection<MongoDocument>(CollectionName);
        _serializer = serializer;
        _expirationGetter = expirationGetter;
    }

    protected abstract string CollectionName { get; }

    public async Task<bool> UpsertAsync(
        string key,
        TValue value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        var filter = Builders<MongoDocument>.Filter.Eq(doc => doc.Id, key);
        var serializedValue = _serializer.Serialize(value);

        var update = Builders<MongoDocument>.Update
            .Set(doc => doc.Value, serializedValue)
            .SetOnInsert(doc => doc.Id, key);

        DateTime? expireAt = null;

        if (_expirationGetter?.Invoke(value) is { } expiration)
        {
            expireAt = expiration;
        }
        else if (expiry.HasValue)
        {
            expireAt = DateTime.UtcNow.Add(expiry.Value);
        }

        update = expireAt.HasValue
            ? update.Set(doc => doc.ExpireAt, expireAt)
            : update.Unset(doc => doc.ExpireAt);

        var options = new UpdateOptions
        {
            IsUpsert = true
        };
        var result = await _collection.UpdateOneAsync(
            filter,
            update,
            options,
            cancellationToken);

        return result.ModifiedCount > 0 || result.UpsertedId != null;
    }

    public async Task<TValue?> ReadAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var filter = Builders<MongoDocument>.Filter.Eq(doc => doc.Id, key);
        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (document == null)
            return default;

        return _serializer.Deserialize(document.Value);
    }

    public async Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var filter = Builders<MongoDocument>.Filter.Eq(doc => doc.Id, key);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}