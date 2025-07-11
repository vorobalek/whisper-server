using Newtonsoft.Json;

namespace Whisper.Extensions;

public static class JsonSerializerSettingsExtensions
{
    public static void AddConverters(
        this JsonSerializerSettings jsonSerializerSettings,
        IEnumerable<JsonConverter> jsonConverters)
    {
        foreach (var jsonConverter in jsonConverters)
            jsonSerializerSettings.Converters.Add(jsonConverter);
    }

    public static string Serialize(
        this JsonSerializerSettings jsonSerializerSettings,
        object @object)
    {
        return JsonConvert.SerializeObject(@object, jsonSerializerSettings);
    }

    public static T Deserialize<T>(
        this JsonSerializerSettings jsonSerializerSettings,
        string @string)
    {
        return JsonConvert.DeserializeObject<T>(@string, jsonSerializerSettings) ??
               throw new InvalidOperationException(
                   $"Failed to deserialize string \"{@string}\" as {typeof(T)}.");
    }
}