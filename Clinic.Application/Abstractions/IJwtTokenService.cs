namespace Clinic.Application.Abstractions;

public interface IJwtTokenService
{
    string CreateToken(string userId, string email, string? fullName, IEnumerable<string> roles);
}
