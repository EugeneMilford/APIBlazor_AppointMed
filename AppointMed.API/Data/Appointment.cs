namespace AppointMed.API.Data
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }
        public string? PatientEmail { get; set; }
        public string? PatientPhoneNumber { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? AppointmentType { get; set; }
        public string? Notes { get; set; }
        public int DoctorId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual Doctor? Doctor { get; set; }
        public virtual Status? Status { get; set; }
        public virtual ApiUser? User { get; set; }
    }
}
