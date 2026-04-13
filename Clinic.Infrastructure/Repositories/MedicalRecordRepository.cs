using Clinic.Application.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories;

public sealed class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly App_Context _context;

    public MedicalRecordRepository(App_Context context)
    {
        _context = context;
    }

    public async Task AddAsync(Medical_Records record, CancellationToken cancellationToken = default)
    {
        await _context.MedicalRecords.AddAsync(record, cancellationToken);
    }

    public async Task<bool> ExistsForAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecords.AnyAsync(m => m.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<Medical_Records?> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecords.AsNoTracking().FirstOrDefaultAsync(m => m.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<Medical_Records?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecords.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Medical_Records?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecords.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Medical_Records>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecords.AsNoTracking().OrderBy(m => m.Id).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Medical_Records>> ListForDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecords
            .AsNoTracking()
            .Where(m => _context.Appointments.Any(a => a.Id == m.AppointmentId && a.DoctorId == doctorId))
            .OrderBy(m => m.Id)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Medical_Records record) => _context.MedicalRecords.Remove(record);
}
