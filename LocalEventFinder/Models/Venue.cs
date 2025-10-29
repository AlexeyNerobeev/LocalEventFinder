using Microsoft.Extensions.Logging;

namespace LocalEventFinder.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
