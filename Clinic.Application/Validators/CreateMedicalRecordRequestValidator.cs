using Clinic.Application.DTOs;
using FluentValidation;

namespace Clinic.Application.Validators;

public sealed class CreateMedicalRecordRequestValidator : AbstractValidator<CreateMedicalRecordRequest>
{
    public CreateMedicalRecordRequestValidator()
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0);
        RuleFor(x => x.Diagnosis).NotEmpty();
        RuleFor(x => x.Prescription).NotEmpty();
        RuleFor(x => x.VisitsNotes).NotEmpty();
    }
}
