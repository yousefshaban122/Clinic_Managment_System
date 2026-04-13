namespace Clinic.Application.DTOs;

public sealed class CreateDoctorRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    public string? UserId { get; set; }
}

public sealed class UpdateDoctorRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    
    public string? UserId { get; set; }
    public bool? IsActive { get; set; }
}

public sealed class DoctorResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    public string? UserId { get; set; }
    public bool IsActive { get; set; }
}
