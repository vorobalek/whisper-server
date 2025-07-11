using MongoDB.Driver;
using Whisper.Data;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage.Mongo.Infrastructure;

namespace Whisper.Storage.Mongo;

internal sealed class SubscriptionMongoStorage(
    IMongoDatabase database,
    IJsonSerializer<Subscription> serializer) :
    MongoStorage<Subscription>(
        database,
        serializer,
        subscription =>
            subscription.ExpirationTime.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(subscription.ExpirationTime.Value).DateTime
                : null),
    ISubscriptionStorage
{
    protected override string CollectionName => MongoCollectionNames.SubscriptionData;
}