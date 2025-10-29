namespace LocalEventFinder.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string Category { get; set; } = string.Empty; 
        public decimal Price { get; set; }
        public int MaxAttendees { get; set; }
        public int VenueId { get; set; }
        public int OrganizerId { get; set; }
        public Venue Venue { get; set; } = null!;
        public Organizer Organizer { get; set; } = null!;
        public ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
    }
}
