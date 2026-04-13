using Clinic.Domain.Entities;

namespace Clinic.Application.Abstractions;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    void Remove(Appointment appointment);
    Task<bool> HasConflictAsync(int doctorId, DateTime appointmentDate, int? excludeAppointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<int?> GetDoctorIdForAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
}
