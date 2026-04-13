namespace Clinic.Application.Abstractions;

public interface IUnitOfWork
{
    IPatientRepository Patients { get; }
    IDoctorRepository Doctors { get; }
    IAppointmentRepository Appointments { get; }
    IInvoiceRepository Invoices { get; }
    IMedicalRecordRepository MedicalRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
