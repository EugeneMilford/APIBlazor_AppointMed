using System.ComponentModel.DataAnnotations;

namespace AppointMed.API.Models.Medicine
{
    public class UpdateMedicineDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [StringLength(100)]
        public string Dosage { get; set; }

        [StringLength(200)]
        public string Manufacturer { get; set; }

        public bool IsAvailable { get; set; }
    }
}
