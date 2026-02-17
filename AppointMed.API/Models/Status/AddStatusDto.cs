using System.ComponentModel.DataAnnotations;

namespace AppointMed.API.Models.Status
{
    public class AddStatusDto
    {
        [Required(ErrorMessage = "Status name is required")]
        [StringLength(50, ErrorMessage = "Status name cannot exceed 50 characters")]
        public string StatusName { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Status description cannot exceed 200 characters")]
        public string? StatusDescription { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a positive number")]
        public int DisplayOrder { get; set; } = 0;
    }
}
