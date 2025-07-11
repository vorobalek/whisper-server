using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Whisper.Services.DbDateTime;

namespace Whisper.Tests.Services.DbDateTime;

[TestClass]
public class DbDateTimeProviderTests
{
    [TestMethod]
    public async Task GetMongoUnixTimeMillisecondsAsync_Should_Return_Correct_Value()
    {
        // Arrange
        var serverNow = DateTime.UtcNow;
        var expected = new DateTimeOffset(serverNow).ToUnixTimeMilliseconds();

        var mongoReply = new BsonDocument
        {
            {
                "system", new BsonDocument
                {
                    {
                        "currentTime", new BsonDateTime(serverNow)
                    }
                }
            }
        };

        var token = new CancellationTokenSource().Token;
        var dbMock = new Mock<IMongoDatabase>(MockBehavior.Strict);
        var sentJson = string.Empty;

        dbMock.Setup<Task<BsonDocument>>(db =>
                db.RunCommandAsync(
                    It.IsAny<Command<BsonDocument>>(),
                    ReadPreference.Primary,
                    token))
            .Callback((Command<BsonDocument> cmd, ReadPreference _, CancellationToken _) =>
                sentJson = cmd.ToJson())
            .ReturnsAsync(mongoReply)
            .Verifiable();

        var provider = new DbDateTimeProvider(dbMock.Object);

        // Act
        var result = await provider.GetMongoUnixTimeMillisecondsAsync(token);

        // Assert
        Assert.AreEqual(expected, result);
        Assert.IsTrue(sentJson.Contains("\"hostInfo\"") && sentJson.Contains("1"), $"Command JSON: {sentJson}");
        dbMock.VerifyAll();
        dbMock.VerifyNoOtherCalls();
    }
}