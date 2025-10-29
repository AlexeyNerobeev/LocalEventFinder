using Microsoft.Extensions.Logging;

namespace LocalEventFinder.Models
{
    public class Organizer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
