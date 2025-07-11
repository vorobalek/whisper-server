namespace Whisper.Services.Serializers.Infrastructure;

public interface IJsonSerializer<T>
{
    string Serialize(T @object);
    T Deserialize(string json);
}