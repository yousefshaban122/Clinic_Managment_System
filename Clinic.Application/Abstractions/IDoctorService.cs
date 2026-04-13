using Clinic.Application.DTOs;

namespace Clinic.Application.Abstractions;

public interface IDoctorService
{
    Task<IReadOnlyList<DoctorResponse>> ListAsync(IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<DoctorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DoctorResponse> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);
    Task<int?> GetDoctorIdForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<DoctorResponse> UpdateAsync(int id, UpdateDoctorRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
}
