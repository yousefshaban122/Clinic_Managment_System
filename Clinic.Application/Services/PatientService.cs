using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services;

public sealed class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;

    public PatientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Patients
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Age = request.Age
        };

        await _unitOfWork.Patients.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (!currentRoles.Contains(Roles.Admin))
            throw new BusinessRuleException("Forbidden.", 403);

        var entity = await _unitOfWork.Patients.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Patient not found.", 404);

        _unitOfWork.Patients.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PatientResponse> GetByIdAsync(int id, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken);
        if (patient is null)
            throw new BusinessRuleException("Patient not found.", 404);

        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
            return Map(patient);

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId is null)
                throw new BusinessRuleException("Doctor profile is not linked to this account.", 403);

            var allowed = await _unitOfWork.Patients.HasAppointmentWithDoctorAsync(id, doctorId.Value, cancellationToken);
            if (!allowed)
                throw new BusinessRuleException("You can only access patients you have appointments with.", 403);

            return Map(patient);
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    public async Task<IReadOnlyList<PatientResponse>> ListAsync(string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (currentRoles.Contains(Roles.Admin) || currentRoles.Contains(Roles.Receptionist))
        {
            var all = await _unitOfWork.Patients.ListAllAsync(cancellationToken);
            return all.Select(Map).ToList();
        }

        if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var doctorId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (doctorId is null)
                throw new BusinessRuleException("Doctor profile is not linked to this account.", 403);

            var list = await _unitOfWork.Patients.ListForDoctorAsync(doctorId.Value, cancellationToken);
            return list.Select(Map).ToList();
        }

        throw new BusinessRuleException("Forbidden.", 403);
    }

    public async Task<PatientResponse> UpdateAsync(int id, UpdatePatientRequest request, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        RequireAdminOrReceptionist(currentRoles);

        var entity = await _unitOfWork.Patients.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Patient not found.", 404);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Address = request.Address;
        entity.Age = request.Age;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private static void RequireAdminOrReceptionist(IReadOnlyList<string> roles)
    {
        if (!roles.Contains(Roles.Admin) && !roles.Contains(Roles.Receptionist))
            throw new BusinessRuleException("Forbidden.", 403);
    }

    private static PatientResponse Map(Patients p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Email = p.Email,
        PhoneNumber = p.PhoneNumber,
        Address = p.Address,
        Age = p.Age
    };
}
