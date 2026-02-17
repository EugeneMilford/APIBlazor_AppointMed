namespace AppointMed.API.Models.Doctor
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Specialization { get; set; }
        public DateTime DateJoined { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Property for display
        public string FullName => $"{FirstName} {LastName}";
    }
}
