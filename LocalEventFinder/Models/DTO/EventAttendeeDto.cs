namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// Data Transfer Object для сущности EventAttendee
    /// </summary>
    public class EventAttendeeDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
    }
}
