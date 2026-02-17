namespace AppointMed.API.Models.Status
{
    public class StatusDto
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public string? StatusDescription { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
