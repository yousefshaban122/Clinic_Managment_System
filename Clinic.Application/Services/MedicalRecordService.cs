using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services;

public sealed class MedicalRecordService : IMedicalRecordService
{
    private readonly IUnitOfWork _unitOfWork;

    public MedicalRecordService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MedicalRecordResponse> CreateAsync(CreateMedicalRecordRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var apptDoctorId = await _unitOfWork.Appointments.GetDoctorIdForAppointmentAsync(request.AppointmentId, cancellationToken);
        if (apptDoctorId is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        await EnsureCanWriteForAppointmentAsync(apptDoctorId.Value, currentUserId, currentRoles, cancellationToken);

        if (await _unitOfWork.MedicalRecords.ExistsForAppointmentAsync(request.AppointmentId, cancellationToken))
            throw new BusinessRuleException("A medical record already exists for this appointment.");

        var entity = new Medical_Records
        {
            AppointmentId = request.AppointmentId,
            Diagnosis = request.Diagnosis,
            Prescription = request.Prescription,
            VisitsNotes = request.VisitsNotes
        };

        await _unitOfWork.MedicalRecords.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (!currentRoles.Contains(Roles.Admin))
            throw new BusinessRuleException("Forbidden.", 403);

        var entity = await _unitOfWork.MedicalRecords.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Medical record not found.", 404);

        _unitOfWork.MedicalRecords.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<MedicalRecordResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.MedicalRecords.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Medical record not found.", 404);

        var apptDoctorId = await _unitOfWork.Appointments.GetDoctorIdForAppointmentAsync(entity.AppointmentId, cancellationToken);
        if (apptDoctorId is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        await EnsureCanReadForAppointmentAsync(apptDoctorId.Value, currentUserId, currentRoles, cancellationToken);

        return Map(entity);
    }

    public async Task<IReadOnlyList<MedicalRecordResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
        {
            var all = await _unitOfWork.MedicalRecords.ListAllAsync(cancellationToken);
            return all.Select(Map).ToList();
        }

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId is null)
                throw new BusinessRuleException("Doctor profile is not linked to this account.", 403);

            var list = await _unitOfWork.MedicalRecords.ListForDoctorAsync(doctorId.Value, cancellationToken);
            return list.Select(Map).ToList();
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    public async Task<MedicalRecordResponse> UpdateAsync(int id, UpdateMedicalRecordRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.MedicalRecords.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Medical record not found.", 404);

        var apptDoctorId = await _unitOfWork.Appointments.GetDoctorIdForAppointmentAsync(entity.AppointmentId, cancellationToken);
        if (apptDoctorId is null)
            throw new BusinessRuleException("Appointment not found.", 404);

        await EnsureCanWriteForAppointmentAsync(apptDoctorId.Value, currentUserId, currentRoles, cancellationToken);

        entity.Diagnosis = request.Diagnosis;
        entity.Prescription = request.Prescription;
        entity.VisitsNotes = request.VisitsNotes;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private async Task EnsureCanReadForAppointmentAsync(int appointmentDoctorId, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
            return;

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId != appointmentDoctorId)
                throw new BusinessRuleException("You can only access medical records for your own appointments.", 403);

            return;
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    private async Task EnsureCanWriteForAppointmentAsync(int appointmentDoctorId, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
            return;

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId != appointmentDoctorId)
                throw new BusinessRuleException("You can only modify medical records for your own appointments.", 403);

            return;
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    private static MedicalRecordResponse Map(Medical_Records m) => new()
    {
        Id = m.Id,
        AppointmentId = m.AppointmentId,
        Diagnosis = m.Diagnosis,
        Prescription = m.Prescription,
        VisitsNotes = m.VisitsNotes
    };
}
