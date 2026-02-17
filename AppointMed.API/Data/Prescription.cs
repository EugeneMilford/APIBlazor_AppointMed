namespace AppointMed.API.Data
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int MedicineId { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
        public DateTime PrescribedDate { get; set; }
        public bool IsFulfilled { get; set; }
        public DateTime? FulfilledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Appointment Appointment { get; set; }
        public Medicine Medicine { get; set; }
        public ApiUser User { get; set; }
    }
}
