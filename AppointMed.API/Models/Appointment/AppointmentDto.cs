namespace AppointMed.API.Models.Appointment
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string? PatientPhoneNumber { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
    }
}