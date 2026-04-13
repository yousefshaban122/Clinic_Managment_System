namespace Clinic.Domain.Entities
{
    public class Doctors
    {
        // Primary identifier
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
       
        public string LastName { get; set; } = string.Empty;
        
        public string FullName => $"{FirstName} {LastName}".Trim();

       
        public string Email { get; set; } = string.Empty;

        public string? Specialization { get; set; }

        public string? UserId { get; set; }
        
        public string? PhoneNumber { get; set; }
       
        public string Address { get; set; } = string.Empty;
   
        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();



    }
}
