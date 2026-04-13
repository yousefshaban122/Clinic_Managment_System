using Clinic.Domain.Entities;

namespace Clinic.Application.Abstractions;

public interface IPatientRepository
{
    Task<Patients?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Patients?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patients>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patients>> ListForDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task AddAsync(Patients patient, CancellationToken cancellationToken = default);
    void Remove(Patients patient);
    Task<bool> HasAppointmentWithDoctorAsync(int patientId, int doctorId, CancellationToken cancellationToken = default);
}
