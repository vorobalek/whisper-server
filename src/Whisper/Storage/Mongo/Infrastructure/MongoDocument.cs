using MongoDB.Bson.Serialization.Attributes;

namespace Whisper.Storage.Mongo.Infrastructure;

[BsonIgnoreExtraElements]
// ReSharper disable once ClassNeverInstantiated.Global
internal record MongoDocument
{
    [BsonId]
    public string Id { get; set; } = null!;

    [BsonElement("value")]
    public string Value { get; set; } = null!;

    [BsonElement("expireAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonIgnoreIfNull]
    public DateTime? ExpireAt { get; set; }
}