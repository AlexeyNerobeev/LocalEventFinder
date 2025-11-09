using LocalEventFinder.Models;

namespace LocalEventFinder.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 30);
        Task<IEnumerable<Event>> GetEventsWithDetailsAsync();
        Task<Event?> GetEventWithDetailsAsync(int id);
        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(int organizerId);
        Task<IEnumerable<Event>> GetEventsByVenueAsync(int venueId);
        Task<bool> IsVenueAvailableAsync(int venueId, DateTime dateTime, int duration);
    }
}
