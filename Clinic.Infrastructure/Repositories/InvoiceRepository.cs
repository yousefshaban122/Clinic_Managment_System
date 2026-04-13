using Clinic.Application.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly App_Context _context;

    public InvoiceRepository(App_Context context)
    {
        _context = context;
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
    }

    public async Task<bool> ExistsForAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.AnyAsync(i => i.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public void Remove(Invoice invoice) => _context.Invoices.Remove(invoice);

    public async Task<IReadOnlyList<Invoice>> ListAsync(int? filterByDoctorId, CancellationToken cancellationToken = default)
    {
        var query = _context.Invoices.AsNoTracking();
        if (filterByDoctorId.HasValue)
        {
            query = query.Where(i =>
                _context.Appointments.Any(a => a.Id == i.AppointmentId && a.DoctorId == filterByDoctorId.Value));
        }

        return await query.OrderByDescending(i => i.Id).ToListAsync(cancellationToken);
    }
}
