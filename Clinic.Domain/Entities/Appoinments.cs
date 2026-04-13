using Clinic.Domain.Enums;

namespace Clinic.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; }

        
        public Doctors Doctor { get; set; } = null!;
        public Patients Patient { get; set; } = null!;

        public Medical_Records? MedicalRecord { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
