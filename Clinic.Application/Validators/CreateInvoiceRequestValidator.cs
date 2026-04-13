using Clinic.Application.DTOs;
using FluentValidation;

namespace Clinic.Application.Validators;

public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0);
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
    }
}
