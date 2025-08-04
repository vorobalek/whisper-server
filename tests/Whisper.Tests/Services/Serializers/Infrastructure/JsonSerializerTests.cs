using Newtonsoft.Json;
using Whisper.Services.Serializers.Infrastructure;

namespace Whisper.Tests.Services.Serializers.Infrastructure;

[TestClass]
public class JsonSerializerTests
{
    [TestMethod]
    public void Serialize_Should_Throw_ArgumentNullException_When_Object_Is_Null()
    {
        // Arrange
        var settings = new JsonSerializerSettings();
        var serializer = new JsonSerializer<object>(settings);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => serializer.Serialize(null!));
    }
}