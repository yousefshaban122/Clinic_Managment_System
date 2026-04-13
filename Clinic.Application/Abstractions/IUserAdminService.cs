using Clinic.Application.DTOs;

namespace Clinic.Application.Abstractions;

public interface IUserAdminService
{
    Task<IReadOnlyList<AdminUserResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<AdminUserResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AdminUserResponse> CreateAsync(AdminCreateUserRequest request, CancellationToken cancellationToken = default);
    Task<AdminUserResponse> UpdateAsync(string id, AdminUpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<AdminUserResponse> SetRolesAsync(string id, AdminSetRolesRequest request, CancellationToken cancellationToken = default);
}
