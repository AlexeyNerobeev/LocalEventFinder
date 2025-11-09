using LocalEventFinder.Models;

namespace LocalEventFinder.Repositories
{
    public interface IEventAttendeeRepository : IRepository<EventAttendee>
    {
        Task<IEnumerable<EventAttendee>> GetAttendeesByEventAsync(int eventId);
        Task<IEnumerable<EventAttendee>> GetAttendeesByEmailAsync(string email);
        Task<int> GetAttendeesCountByEventAsync(int eventId);
        Task<bool> IsUserRegisteredForEventAsync(string email, int eventId);
    }
}
