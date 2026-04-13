using Clinic.Application.DTOs;

namespace Clinic.Application.Abstractions;

public interface IPatientService
{
    Task<IReadOnlyList<PatientResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default);
    Task<PatientResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<PatientResponse> UpdateAsync(int id, UpdatePatientRequest request, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
}
