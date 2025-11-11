namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для детальной информации о событии
    /// </summary>
    public class EventDetailsDto
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
        public VenueDto Venue { get; set; } = null!;
        public OrganizerDto Organizer { get; set; } = null!;
        public List<EventAttendeeDto> Attendees { get; set; } = new();
        public bool IsUpcoming => DateTime > DateTime.UtcNow;
        public bool HasAvailableSpots => CurrentAttendees < MaxAttendees;
    }
}
