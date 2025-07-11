using FluentValidation;

namespace Whisper.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<TEntity, string?> Base64String<TEntity>(
        this IRuleBuilderOptions<TEntity, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(base64String =>
            {
                if (string.IsNullOrWhiteSpace(base64String))
                    return false;

                var buffer = new Span<byte>(new byte[base64String.Length]);
                return Convert.TryFromBase64String(base64String, buffer, out _);
            });
    }
}