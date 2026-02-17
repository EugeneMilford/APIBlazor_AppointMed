using System.ComponentModel.DataAnnotations;

namespace AppointMed.API.Models.Prescription
{
    public class AddPrescriptionDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        [StringLength(1000)]
        public string Instructions { get; set; }
    }
}
