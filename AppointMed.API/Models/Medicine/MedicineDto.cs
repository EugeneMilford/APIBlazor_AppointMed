namespace AppointMed.API.Models.Medicine
{
    public class MedicineDto
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Dosage { get; set; }
        public string Manufacturer { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
