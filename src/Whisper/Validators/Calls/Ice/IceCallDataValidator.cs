using FluentValidation;
using Whisper.Models.Calls.Ice;
using Whisper.Validators.Calls.Infrastructure;

namespace Whisper.Validators.Calls.Ice;

internal sealed class IceCallDataValidator : EncryptedCallDataValidator<IceCallData>
{
    public IceCallDataValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Direction)
            .NotEqual(IceDirection.Unknown).WithMessage("Direction is invalid.");
    }
}