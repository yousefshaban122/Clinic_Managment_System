using Clinic.Application.Abstractions;
using Clinic.Infrastructure.data;

namespace Clinic.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly App_Context _context;

    public UnitOfWork(App_Context context)
    {
        _context = context;
        Patients = new PatientRepository(context);
        Doctors = new DoctorRepository(context);
        Appointments = new AppointmentRepository(context);
        Invoices = new InvoiceRepository(context);
        MedicalRecords = new MedicalRecordRepository(context);
    }

    public IPatientRepository Patients { get; }
    public IDoctorRepository Doctors { get; }
    public IAppointmentRepository Appointments { get; }
    public IInvoiceRepository Invoices { get; }
    public IMedicalRecordRepository MedicalRecords { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
