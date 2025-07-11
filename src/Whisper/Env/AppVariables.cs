using Whisper.Extensions;

namespace Whisper.Env;

public static class AppVariables
{
    public static string Port => "PORT".GetEnvironmentVariableWithFallbackValue("80");

    public static string[] CorsOrigins => "CORS".GetEnvironmentVariableWithFallbackValue(string.Empty)
        .Trim()
        .Split(',', ';', ' ')
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToArray();

    public static string? PathBase => "PATH_BASE".GetEnvironmentVariable();

    public static string RedisConnectionString =>
        "REDIS_CONNECTION_STRING".GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static string MongoConnectionString =>
        "MONGO_CONNECTION_STRING".GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();
}