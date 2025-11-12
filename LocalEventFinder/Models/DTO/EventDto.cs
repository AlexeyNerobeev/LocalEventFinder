namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// Data Transfer Object для сущности Event
    /// </summary>
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxAttendees { get; set; }
        public int CurrentAttendees { get; set; }
        public int VenueId { get; set; }
        public int OrganizerId { get; set; }
        public bool IsUpcoming => DateTime > DateTime.UtcNow;
        public bool HasAvailableSpots => CurrentAttendees < MaxAttendees;
    }
}
