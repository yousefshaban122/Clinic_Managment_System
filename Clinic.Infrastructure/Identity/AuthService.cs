using Clinic.Application.Abstractions;
using Clinic.Application.Common;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Clinic.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Clinic.Infrastructure.Identity;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new BusinessRuleException("Invalid email or password.", 401);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.CreateToken(user.Id, user.Email!, user.FullName, roles);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            UserId = user.Id,
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Role == Roles.Admin)
            throw new BusinessRuleException("Cannot register as Admin through this endpoint.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.Role == Roles.Doctor
                ? $"{request.FirstName} {request.LastName}".Trim()
                : request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);

        if (request.Role == Roles.Doctor)
        {
            var doctor = new Doctors
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Email = request.Email,
                Specialization = request.Specialization,
                UserId = user.Id,
                Address = "N/A",
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.CreateToken(user.Id, user.Email!, user.FullName, roles);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            UserId = user.Id,
            Roles = roles.ToList()
        };
    }
}
