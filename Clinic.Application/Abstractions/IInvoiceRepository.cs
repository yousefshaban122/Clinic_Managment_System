using Clinic.Domain.Entities;

namespace Clinic.Application.Abstractions;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    void Remove(Invoice invoice);
    Task<bool> ExistsForAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> ListAsync(int? filterByDoctorId, CancellationToken cancellationToken = default);
}
