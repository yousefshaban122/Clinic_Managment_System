using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services;

public sealed class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (currentRoles.Contains(Roles.Doctor) &&
            !currentRoles.Contains(Roles.Admin) &&
            !currentRoles.Contains(Roles.Receptionist))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var myDoctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (myDoctorId != request.DoctorId)
                throw new BusinessRuleException("You can only create appointments for your own doctor profile.", 403);
        }

        if (request.AppointmentDate <= DateTime.UtcNow)
            throw new BusinessRuleException("Appointment must be in the future.");

        if (!await _unitOfWork.Doctors.ExistsAsync(request.DoctorId, cancellationToken))
            throw new BusinessRuleException("Doctor not found.", 404);

        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            throw new BusinessRuleException("Patient not found.", 404);

        var conflict = await _unitOfWork.Appointments.HasConflictAsync(
            request.DoctorId,
            request.AppointmentDate,
            excludeAppointmentId: null,
            cancellationToken);

        if (conflict)
            throw new BusinessRuleException("This doctor already has an appointment at the same time.");

        var entity = new Appointment
        {
            DoctorId = request.DoctorId,
            PatientId = request.PatientId,
            AppointmentDate = request.AppointmentDate,
            Reason = request.Reason,
            Status = AppointmentStatus.Pending
        };

        await _unitOfWork.Appointments.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task DeleteAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        await EnsureCanAccessAppointmentAsync(entity, currentUserId, currentRoles, cancellationToken);

        _unitOfWork.Appointments.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AppointmentResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        await EnsureCanAccessAppointmentAsync(entity, currentUserId, currentRoles, cancellationToken);

        return Map(entity);
    }

    public async Task<IReadOnlyList<AppointmentResponse>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Doctors.ExistsAsync(doctorId, cancellationToken))
            throw new BusinessRuleException("Doctor not found.", 404);

        var list = await _unitOfWork.Appointments.GetByDoctorAsync(doctorId, cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<AppointmentResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
        {
            var all = await _unitOfWork.Appointments.ListAllAsync(cancellationToken);
            return all.Select(Map).ToList();
        }

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId is null)
                throw new BusinessRuleException("Doctor profile is not linked to this account.", 403);

            return await GetByDoctorAsync(doctorId.Value, cancellationToken);
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    public async Task<AppointmentResponse> UpdateAsync(int id, UpdateAppointmentRequest request, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (!currentRoles.Contains(Roles.Admin) && !currentRoles.Contains(Roles.Receptionist))
            throw new BusinessRuleException("Forbidden.", 403);

        var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        if (entity.Status == AppointmentStatus.Completed && !currentRoles.Contains(Roles.Admin))
            throw new BusinessRuleException("Cannot reschedule a completed appointment.");

        if (request.AppointmentDate <= DateTime.UtcNow)
            throw new BusinessRuleException("Appointment must be in the future.");

        if (!await _unitOfWork.Doctors.ExistsAsync(request.DoctorId, cancellationToken))
            throw new BusinessRuleException("Doctor not found.", 404);

        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            throw new BusinessRuleException("Patient not found.", 404);

        var conflict = await _unitOfWork.Appointments.HasConflictAsync(
            request.DoctorId,
            request.AppointmentDate,
            excludeAppointmentId: id,
            cancellationToken);

        if (conflict)
            throw new BusinessRuleException("This doctor already has an appointment at the same time.");

        entity.PatientId = request.PatientId;
        entity.DoctorId = request.DoctorId;
        entity.AppointmentDate = request.AppointmentDate;
        entity.Reason = request.Reason;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<AppointmentResponse> UpdateStatusAsync(int id, UpdateAppointmentStatusRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        await EnsureCanAccessAppointmentAsync(entity, currentUserId, currentRoles, cancellationToken);

        entity.Status = request.Status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private async Task EnsureCanAccessAppointmentAsync(Appointment appointment, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
            return;

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId != appointment.DoctorId)
                throw new BusinessRuleException("You can only access your own appointments.", 403);

            return;
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    private static AppointmentResponse Map(Appointment a) => new()
    {
        Id = a.Id,
        PatientId = a.PatientId,
        DoctorId = a.DoctorId,
        AppointmentDate = a.AppointmentDate,
        Reason = a.Reason,
        Status = a.Status
    };
}
