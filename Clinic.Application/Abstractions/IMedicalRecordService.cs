using Clinic.Application.DTOs;

namespace Clinic.Application.Abstractions;

public interface IMedicalRecordService
{
    Task<IReadOnlyList<MedicalRecordResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<MedicalRecordResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<MedicalRecordResponse> CreateAsync(CreateMedicalRecordRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<MedicalRecordResponse> UpdateAsync(int id, UpdateMedicalRecordRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
}
