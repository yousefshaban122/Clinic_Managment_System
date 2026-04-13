using Clinic.Domain.Entities;

namespace Clinic.Application.Abstractions;

public interface IMedicalRecordRepository
{
    Task<Medical_Records?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Medical_Records?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Medical_Records?> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Medical_Records>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Medical_Records>> ListForDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task AddAsync(Medical_Records record, CancellationToken cancellationToken = default);
    void Remove(Medical_Records record);
    Task<bool> ExistsForAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
}
