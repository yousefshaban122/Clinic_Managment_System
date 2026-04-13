using Clinic.Application.DTOs;

namespace Clinic.Application.Abstractions;

public interface IAppointmentService
{
    Task<IReadOnlyList<AppointmentResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentResponse>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> UpdateAsync(int id, UpdateAppointmentRequest request, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<AppointmentResponse> UpdateStatusAsync(int id, UpdateAppointmentStatusRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
}
