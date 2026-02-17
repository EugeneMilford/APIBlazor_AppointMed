namespace AppointMed.API.Data
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Specialization { get; set; }
        public string? Bio { get; set; }
        public Uri? ProfileImageUrl { get; set; }
        public DateTime DateJoined { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Appointment>? Appointments { get; set; }

        public string FullName => $"{FirstName ?? ""} {LastName ?? ""}".Trim();
    }
}
