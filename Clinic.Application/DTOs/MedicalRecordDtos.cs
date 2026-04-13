namespace Clinic.Application.DTOs;

public sealed class CreateMedicalRecordRequest
{
    public int AppointmentId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string VisitsNotes { get; set; } = string.Empty;
}

public sealed class UpdateMedicalRecordRequest
{
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string VisitsNotes { get; set; } = string.Empty;
}

public sealed class MedicalRecordResponse
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string VisitsNotes { get; set; } = string.Empty;
}
