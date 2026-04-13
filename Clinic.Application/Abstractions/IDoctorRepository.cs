using Clinic.Domain.Entities;

namespace Clinic.Application.Abstractions;

public interface IDoctorRepository
{
    Task<Doctors?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Doctors?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctors>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctors>> ListAllForAdminAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Doctors doctor, CancellationToken cancellationToken = default);
    void Remove(Doctors doctor);
    Task<int?> GetDoctorIdByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
}
