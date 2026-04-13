using Clinic.Application.DTOs;
using FluentValidation;

namespace Clinic.Application.Validators;

public sealed class UpdateInvoiceRequestValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceRequestValidator()
    {
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
    }
}
