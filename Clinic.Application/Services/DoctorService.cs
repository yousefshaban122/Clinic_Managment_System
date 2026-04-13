using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services;

public sealed class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;

    public DoctorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DoctorResponse> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Doctors
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            DateOfBirth = request.DateOfBirth,
            
            UserId = request.UserId,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Doctors.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task DeleteAsync(int id, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        if (!currentRoles.Contains(Roles.Admin))
            throw new BusinessRuleException("Forbidden.", 403);

        var count = await _unitOfWork.Doctors.CountAppointmentsAsync(id, cancellationToken);
        if (count > 0)
            throw new BusinessRuleException("Cannot delete a doctor who has appointments.");

        var entity = await _unitOfWork.Doctors.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Doctor not found.", 404);

        _unitOfWork.Doctors.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DoctorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Doctors.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Doctor not found.", 404);

        return Map(entity);
    }

    public Task<int?> GetDoctorIdForUserAsync(string userId, CancellationToken cancellationToken = default) =>
        _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(userId, cancellationToken);

    public async Task<IReadOnlyList<DoctorResponse>> ListAsync(IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var list = currentRoles.Contains(Roles.Admin)
            ? await _unitOfWork.Doctors.ListAllForAdminAsync(cancellationToken)
            : await _unitOfWork.Doctors.ListAsync(cancellationToken);

        return list.Select(Map).ToList();
    }

    public async Task<DoctorResponse> UpdateAsync(int id, UpdateDoctorRequest request, string? currentUserId, IReadOnlyList<string> currentRoles, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Doctors.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new BusinessRuleException("Doctor not found.", 404);

        if (currentRoles.Contains(Roles.Admin))
        {
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.Email = request.Email;
            entity.PhoneNumber = request.PhoneNumber;
            entity.Address = request.Address;
            entity.DateOfBirth = request.DateOfBirth;
            if (request.UserId is not null)
                entity.UserId = request.UserId;
            if (request.IsActive.HasValue)
                entity.IsActive = request.IsActive.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else if (currentRoles.Contains(Roles.Doctor))
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new BusinessRuleException("Forbidden.", 403);

            var linkedId = await _unitOfWork.Doctors.GetDoctorIdByUserIdAsync(currentUserId, cancellationToken);
            if (linkedId != id)
                throw new BusinessRuleException("You can only update your own doctor profile.", 403);

            if (request.UserId is not null || request.IsActive.HasValue)
                throw new BusinessRuleException("You cannot change account linkage or active status.", 403);

            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.Email = request.Email;
            entity.PhoneNumber = request.PhoneNumber;
            entity.Address = request.Address;
            entity.DateOfBirth = request.DateOfBirth;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            throw new BusinessRuleException("Forbidden.", 403);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    private static DoctorResponse Map(Doctors d) => new()
    {
        Id = d.Id,
        FirstName = d.FirstName,
        LastName = d.LastName,
        Email = d.Email,
        PhoneNumber = d.PhoneNumber,
        Address = d.Address,
        DateOfBirth = d.DateOfBirth,
       
        UserId = d.UserId,
        IsActive = d.IsActive
    };
}
