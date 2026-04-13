using Clinic.Application.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories;

public sealed class DoctorRepository : IDoctorRepository
{
    private readonly App_Context _context;

    public DoctorRepository(App_Context context)
    {
        _context = context;
    }

    public async Task AddAsync(Doctors doctor, CancellationToken cancellationToken = default)
    {
        await _context.Doctors.AddAsync(doctor, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.AnyAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Doctors?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Doctors?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public void Remove(Doctors doctor) => _context.Doctors.Remove(doctor);

    public async Task<int> CountAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.CountAsync(a => a.DoctorId == doctorId, cancellationToken);
    }

    public async Task<int?> GetDoctorIdByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .Where(d => d.UserId == userId)
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Doctors>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Doctors>> ListAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync(cancellationToken);
    }
}
