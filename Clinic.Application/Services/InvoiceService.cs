using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TotalAmount < 0)
            throw new BusinessRuleException("Total amount cannot be negative.");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        if (appointment.Status != AppointmentStatus.Completed)
            throw new BusinessRuleException("Invoice can only be created after the appointment is completed.");

        if (await _unitOfWork.Invoices.ExistsForAppointmentAsync(request.AppointmentId, cancellationToken))
            throw new BusinessRuleException("An invoice already exists for this appointment.");

        var invoice = new Invoice
        {
            AppointmentId = request.AppointmentId,
            TotalAmount = request.TotalAmount,
            Status = request.Status
        };

        await _unitOfWork.Invoices.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(invoice);
    }

    public async Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (!currentRoles.Contains(Roles.Admin))
            throw new BusinessRuleException("Forbidden.", 403);

        var entity = await _unitOfWork.Invoices.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Invoice not found.", 404);

        _unitOfWork.Invoices.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<InvoiceResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
            throw new BusinessRuleException("Invoice not found.", 404);

        var doctorId = await _unitOfWork.Appointments.GetDoctorIdForAppointmentAsync(invoice.AppointmentId, cancellationToken);
        if (doctorId is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
            return Map(invoice);

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var myDoctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (myDoctorId != doctorId)
                throw new BusinessRuleException("You can only access invoices for your own appointments.", 403);

            return Map(invoice);
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    public async Task<IReadOnlyList<InvoiceResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
        {
            var all = await _unitOfWork.Invoices.ListAsync(filterByDoctorId: null, cancellationToken);
            return all.Select(Map).ToList();
        }

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId is null)
                throw new BusinessRuleException("Doctor profile is not linked to this account.", 403);

            var list = await _unitOfWork.Invoices.ListAsync(doctorId, cancellationToken);
            return list.Select(Map).ToList();
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    public async Task<InvoiceResponse> UpdateAsync(int id, UpdateInvoiceRequest request, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Invoices.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Invoice not found.", 404);

        if (currentRoles.Contains(Roles.Admin))
        {
            if (request.TotalAmount < 0)
                throw new BusinessRuleException("Total amount cannot be negative.");
            entity.TotalAmount = request.TotalAmount;
            entity.Status = request.Status;
        }
        else if (currentRoles.Contains(Roles.Receptionist))
        {
            if (request.TotalAmount != entity.TotalAmount)
                throw new BusinessRuleException("Only an administrator can change the invoice total.");
            entity.Status = request.Status;
        }
        else
        {
            throw new BusinessRuleException("Forbidden.", 403);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private static InvoiceResponse Map(Invoice i) => new()
    {
        Id = i.Id,
        AppointmentId = i.AppointmentId,
        TotalAmount = i.TotalAmount,
        Status = i.Status
    };
}
