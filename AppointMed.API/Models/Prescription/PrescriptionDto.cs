namespace AppointMed.API.Models.Prescription
{
    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public decimal MedicinePrice { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public string Instructions { get; set; }
        public DateTime PrescribedDate { get; set; }
        public bool IsFulfilled { get; set; }
        public DateTime? FulfilledDate { get; set; }
    }
}
