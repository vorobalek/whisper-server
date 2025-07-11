using System.Text;
using FluentValidation;
using Whisper.Extensions;
using Whisper.Models.Calls.Answer;
using Whisper.Models.Calls.Close;
using Whisper.Models.Calls.Dial;
using Whisper.Models.Calls.Ice;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Models.Calls.Offer;
using Whisper.Models.Calls.Update;
using Whisper.Services.Cryptography;
using Whisper.Services.Serializers.Infrastructure;

namespace Whisper.Validators.Calls.Infrastructure;

internal sealed class CallRequestValidator : AbstractValidator<ICallRequest>
{
    public CallRequestValidator(
        IValidator<UpdateCallRequest> updateCallRequestValidator,
        IValidator<DialCallRequest> dialCallRequestValidator,
        IValidator<OfferCallRequest> offerCallRequestValidator,
        IValidator<AnswerCallRequest> answerCallRequestValidator,
        IValidator<IceCallRequest> iceCallRequestValidator,
        IValidator<CloseCallRequest> closeCallRequestValidator)
    {
        RuleFor(x => x)
            .SetInheritanceValidator(x =>
            {
                x.Add(updateCallRequestValidator);
                x.Add(dialCallRequestValidator);
                x.Add(offerCallRequestValidator);
                x.Add(answerCallRequestValidator);
                x.Add(iceCallRequestValidator);
                x.Add(closeCallRequestValidator);
            });
    }
}

internal class CallRequestValidator<TRequest, TData> : AbstractValidator<TRequest>
    where TRequest : class, ICallRequest<TData>
    where TData : class, ICallData
{
    public CallRequestValidator(
        IJsonSerializer<ICallData> callDataSerializer,
        IValidator<TData> callDataValidator,
        ICrypto crypto)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Method cannot be empty.");

        RuleFor(x => x.Data)
            .SetValidator(callDataValidator);

        RuleFor(x => x.Signature)
            .NotEmpty().WithMessage("Signature cannot be empty.")
            .Base64String().WithMessage("Signature must be base64 string.");

        RuleFor(x => x.Signature)
            .Must((request, _) =>
            {
                var publicKeyBytes = Convert.FromBase64String(request.Data.PublicKey);
                var signatureBytes = Convert.FromBase64String(request.Signature);
                var message = callDataSerializer.Serialize(request.Data);
                var messageBytes = Encoding.UTF8.GetBytes(message);
                return crypto.VerifySignature(publicKeyBytes, messageBytes, signatureBytes);
            }).WithMessage("Invalid signature.");
    }
}