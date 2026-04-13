using Clinic.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Domain.Entities
{
        public class Invoice
        {
            public int Id { get; set; }

            public int AppointmentId { get; set; }

            public decimal TotalAmount { get; set; }

            public PaymentStatus Status { get; set; }

             [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }
    }
    
}
