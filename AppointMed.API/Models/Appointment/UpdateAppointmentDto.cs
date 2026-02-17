using System.ComponentModel.DataAnnotations;

namespace AppointMed.API.Models.Appointment
{
    public class UpdateAppointmentDto
    {
        [Required]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PatientFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PatientLastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string PatientEmail { get; set; } = string.Empty;

        [StringLength(15)]
        public string? PatientPhoneNumber { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        [StringLength(100)]
        public string AppointmentType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }
    }
}