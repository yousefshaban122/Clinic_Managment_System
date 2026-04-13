using Clinic.Application.DTOs;
using FluentValidation;

namespace Clinic.Application.Validators;

public sealed class UpdateMedicalRecordRequestValidator : AbstractValidator<UpdateMedicalRecordRequest>
{
    public UpdateMedicalRecordRequestValidator()
    {
        RuleFor(x => x.Diagnosis).NotEmpty();
        RuleFor(x => x.Prescription).NotEmpty();
        RuleFor(x => x.VisitsNotes).NotEmpty();
    }
}
