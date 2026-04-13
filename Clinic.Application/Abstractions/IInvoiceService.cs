using Clinic.Application.DTOs;

namespace Clinic.Application.Abstractions;

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> UpdateAsync(int id, UpdateInvoiceRequest request, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default);
}
