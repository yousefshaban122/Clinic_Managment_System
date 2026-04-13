using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Application.Services;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Moq;
using Xunit;

namespace Clinic.Application.Tests;

public sealed class AppointmentServiceTests
{
    private static Mock<IUnitOfWork> CreateUow(
        Mock<IAppointmentRepository>? appointments = null,
        Mock<IDoctorRepository>? doctors = null,
        Mock<IPatientRepository>? patients = null)
    {
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Appointments).Returns((appointments ?? new Mock<IAppointmentRepository>()).Object);
        uow.SetupGet(x => x.Doctors).Returns((doctors ?? new Mock<IDoctorRepository>()).Object);
        uow.SetupGet(x => x.Patients).Returns((patients ?? new Mock<IPatientRepository>()).Object);
        uow.SetupGet(x => x.Invoices).Returns(new Mock<IInvoiceRepository>().Object);
        return uow;
    }

    [Fact]
    public async Task CreateAsync_Throws_When_Appointment_Not_In_Future()
    {
        var uow = CreateUow();
        var sut = new AppointmentService(uow.Object);
        var request = new CreateAppointmentRequest
        {
            DoctorId = 1,
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddMinutes(-5),
            Reason = "Checkup"
        };

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            sut.CreateAsync(request, null, new List<string> { "Admin" }));
        Assert.Contains("future", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_Throws_When_Double_Booking()
    {
        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(a => a.HasConflictAsync(1, It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var doctors = new Mock<IDoctorRepository>();
        doctors.Setup(d => d.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var patients = new Mock<IPatientRepository>();
        patients.Setup(p => p.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Patients { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.c", Address = "x", Age = 30 });

        var uow = CreateUow(appointments, doctors, patients);
        var sut = new AppointmentService(uow.Object);

        var request = new CreateAppointmentRequest
        {
            DoctorId = 1,
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Reason = "Checkup"
        };

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            sut.CreateAsync(request, null, new List<string> { "Admin" }));
        Assert.Contains("same time", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_Succeeds_When_Valid()
    {
        var future = DateTime.UtcNow.AddDays(2);

        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(a => a.HasConflictAsync(2, future, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var doctors = new Mock<IDoctorRepository>();
        doctors.Setup(d => d.ExistsAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var patients = new Mock<IPatientRepository>();
        patients.Setup(p => p.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Patients { Id = 3, FirstName = "A", LastName = "B", Email = "a@b.c", Address = "x", Age = 25 });

        var uow = CreateUow(appointments, doctors, patients);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = new AppointmentService(uow.Object);
        var request = new CreateAppointmentRequest
        {
            DoctorId = 2,
            PatientId = 3,
            AppointmentDate = future,
            Reason = "Follow-up"
        };

        var result = await sut.CreateAsync(request, currentUserId: null, currentRoles: new List<string> { "Admin" });

        Assert.Equal(AppointmentStatus.Pending, result.Status);
        Assert.Equal(2, result.DoctorId);
        Assert.Equal(3, result.PatientId);
        appointments.Verify(a => a.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
