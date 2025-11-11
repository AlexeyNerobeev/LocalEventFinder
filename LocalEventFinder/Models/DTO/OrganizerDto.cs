namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// Data Transfer Object для сущности Organizer
    /// </summary>
    public class OrganizerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int EventsCount { get; set; }
    }
}
