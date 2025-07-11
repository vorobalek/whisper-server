using Whisper.Extensions;

namespace Whisper.Env;

public static class NotificationVariables
{
    public static readonly string Subject = "NOTIFICATION_SUBJECT".GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly string
        PublicKey = "NOTIFICATION_PUBLIC_KEY".GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly string PrivateKey =
        "NOTIFICATION_PRIVATE_KEY".GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();
}