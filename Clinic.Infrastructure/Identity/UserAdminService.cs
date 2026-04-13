using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Infrastructure.data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Identity;

public sealed class UserAdminService : IUserAdminService
{
    private static readonly string[] AllowedRoles = { Roles.Admin, Roles.Doctor, Roles.Receptionist };

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly App_Context _db;

    public UserAdminService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        App_Context db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task<AdminUserResponse> CreateAsync(AdminCreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRoles(request.Roles);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(" ", result.Errors.Select(e => e.Description)));

        foreach (var role in request.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
            await _userManager.AddToRoleAsync(user, role);
        }

        return await MapAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            throw new BusinessRuleException("User not found.", 404);

        var doctors = await _db.Doctors.Where(d => d.UserId == id).ToListAsync(cancellationToken);
        foreach (var d in doctors)
        {
            d.UserId = null;
            d.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        var del = await _userManager.DeleteAsync(user);
        if (!del.Succeeded)
            throw new BusinessRuleException(string.Join(" ", del.Errors.Select(e => e.Description)));
    }

    public async Task<AdminUserResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            throw new BusinessRuleException("User not found.", 404);

        return await MapAsync(user, cancellationToken);
    }

    public async Task<IReadOnlyList<AdminUserResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync(cancellationToken);
        var list = new List<AdminUserResponse>();
        foreach (var u in users)
            list.Add(await MapAsync(u, cancellationToken));
        return list;
    }

    public async Task<AdminUserResponse> SetRolesAsync(string id, AdminSetRolesRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRoles(request.Roles);

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            throw new BusinessRuleException("User not found.", 404);

        var current = await _userManager.GetRolesAsync(user);
        var remove = await _userManager.RemoveFromRolesAsync(user, current);
        if (!remove.Succeeded)
            throw new BusinessRuleException(string.Join(" ", remove.Errors.Select(e => e.Description)));

        foreach (var role in request.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
            await _userManager.AddToRoleAsync(user, role);
        }

        return await MapAsync(user, cancellationToken);
    }

    public async Task<AdminUserResponse> UpdateAsync(string id, AdminUpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            throw new BusinessRuleException("User not found.", 404);

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(request.Email) && !string.Equals(request.Email, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = request.Email;
            user.UserName = request.Email;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(" ", result.Errors.Select(e => e.Description)));

        return await MapAsync(user, cancellationToken);
    }

    private static void ValidateRoles(IReadOnlyList<string> roles)
    {
        foreach (var r in roles)
        {
            if (!AllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
                throw new BusinessRuleException($"Role '{r}' is not allowed.");
        }
    }

    private async Task<AdminUserResponse> MapAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        return new AdminUserResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Roles = roleNames.ToList(),
            LockoutEnabled = user.LockoutEnabled
        };
    }
}
