using Clinic.Application.DTOs;
using FluentValidation;

namespace Clinic.Application.Validators;

public sealed class AdminSetRolesRequestValidator : AbstractValidator<AdminSetRolesRequest>
{
    public AdminSetRolesRequestValidator()
    {
        RuleFor(x => x.Roles).NotEmpty().Must(r => r.Count > 0).WithMessage("At least one role is required.");
    }
}
