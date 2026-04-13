using Clinic.Domain.Enums;

namespace Clinic.Application.DTOs;

public sealed class CreateInvoiceRequest
{
    public int AppointmentId { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
}

public sealed class UpdateInvoiceRequest
{
    public decimal TotalAmount { get; set; }
    public PaymentStatus Status { get; set; }
}

public sealed class InvoiceResponse
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus Status { get; set; }
}
