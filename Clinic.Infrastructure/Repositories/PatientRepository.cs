using Clinic.Application.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories;

public sealed class PatientRepository : IPatientRepository
{
    private readonly App_Context _context;

    public PatientRepository(App_Context context)
    {
        _context = context;
    }

    public async Task AddAsync(Patients patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
    }

    public async Task<Patients?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Patients?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Remove(Patients patient) => _context.Patients.Remove(patient);

    public async Task<IReadOnlyList<Patients>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Patients>> ListForDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.Appointments.Any(a => a.DoctorId == doctorId))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAppointmentWithDoctorAsync(int patientId, int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.AnyAsync(
            a => a.PatientId == patientId && a.DoctorId == doctorId,
            cancellationToken);
    }
}
