namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для создания нового события
    /// </summary>
    public class CreateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxAttendees { get; set; }
        public int VenueId { get; set; }
        public int OrganizerId { get; set; }
    }
}
