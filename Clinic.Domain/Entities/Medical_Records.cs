using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Domain.Entities
{
    public class Medical_Records
    {
        [Column("id")]
        public int Id { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Prescription { get; set; } = string.Empty;
        public string VisitsNotes { get; set; } = string.Empty;

        // FK to appointment (one-to-one)
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }
    }
}
