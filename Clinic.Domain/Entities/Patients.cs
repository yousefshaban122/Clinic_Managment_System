using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Domain.Entities
{
    public class Patients
    {
        [Column("id")]
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
       
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; } = string.Empty;
         
        public string? PhoneNumber { get; set; }
        
        public string Address { get; set; } = string.Empty;
        [Column("age")]
        public int Age { get; set; }

        
        public List<Appointment> Appointments { get; set; } = new();
    }
}
