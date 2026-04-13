using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using FluentValidation;

namespace Clinic.Application.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty();

        RuleFor(x => x.Role)
            .Must(r => r is Roles.Receptionist or Roles.Doctor)
            .WithMessage($"Role must be {Roles.Receptionist} or {Roles.Doctor}.");

        When(x => x.Role == Roles.Doctor, () =>
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Specialization).NotEmpty();
        });

        When(x => x.Role == Roles.Receptionist, () =>
        {
            RuleFor(x => x.FullName).NotEmpty();
        });
    }
}
