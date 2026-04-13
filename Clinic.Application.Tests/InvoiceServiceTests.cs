using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Application.Services;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Moq;
using Xunit;

namespace Clinic.Application.Tests;

public sealed class InvoiceServiceTests
{
    [Fact]
    public async Task CreateAsync_Throws_When_Appointment_Not_Completed()
    {
        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(a => a.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Appointment
            {
                Id = 10,
                DoctorId = 1,
                PatientId = 2,
                AppointmentDate = DateTime.UtcNow,
                Reason = "x",
                Status = AppointmentStatus.Pending
            });

        var invoices = new Mock<IInvoiceRepository>();

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Appointments).Returns(appointments.Object);
        uow.SetupGet(x => x.Invoices).Returns(invoices.Object);
        uow.SetupGet(x => x.Patients).Returns(new Mock<IPatientRepository>().Object);
        uow.SetupGet(x => x.Doctors).Returns(new Mock<IDoctorRepository>().Object);

        var sut = new InvoiceService(uow.Object);
        var request = new CreateInvoiceRequest { AppointmentId = 10, TotalAmount = 100m };

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => sut.CreateAsync(request));
        Assert.Contains("completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_Succeeds_For_Completed_Appointment_With_Total()
    {
        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(a => a.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Appointment
            {
                Id = 5,
                DoctorId = 1,
                PatientId = 2,
                AppointmentDate = DateTime.UtcNow,
                Reason = "x",
                Status = AppointmentStatus.Completed
            });

        var invoices = new Mock<IInvoiceRepository>();
        invoices.Setup(i => i.ExistsForAppointmentAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Appointments).Returns(appointments.Object);
        uow.SetupGet(x => x.Invoices).Returns(invoices.Object);
        uow.SetupGet(x => x.Patients).Returns(new Mock<IPatientRepository>().Object);
        uow.SetupGet(x => x.Doctors).Returns(new Mock<IDoctorRepository>().Object);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = new InvoiceService(uow.Object);
        var request = new CreateInvoiceRequest
        {
            AppointmentId = 5,
            TotalAmount = 250.50m,
            Status = PaymentStatus.Unpaid
        };

        var result = await sut.CreateAsync(request);

        Assert.Equal(5, result.AppointmentId);
        Assert.Equal(250.50m, result.TotalAmount);
        invoices.Verify(i => i.AddAsync(It.Is<Invoice>(inv => inv.TotalAmount == 250.50m), It.IsAny<CancellationToken>()), Times.Once);
    }
}
