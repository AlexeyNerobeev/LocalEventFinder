using LocalEventFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalEventFinder.Repositories
{
    public class EventAttendeeRepository : Repository<EventAttendee>, IEventAttendeeRepository
    {
        public EventAttendeeRepository(EventDbContext context) : base(context) { }

        public async Task<IEnumerable<EventAttendee>> GetAttendeesByEventAsync(int eventId)
        {
            return await _dbSet
                .Where(ea => ea.EventId == eventId)
                .OrderBy(ea => ea.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventAttendee>> GetAttendeesByEmailAsync(string email)
        {
            return await _dbSet
                .Where(ea => ea.Email == email)
                .Include(ea => ea.Event)
                .OrderByDescending(ea => ea.RegistrationDate)
                .ToListAsync();
        }

        public async Task<int> GetAttendeesCountByEventAsync(int eventId)
        {
            return await _dbSet
                .CountAsync(ea => ea.EventId == eventId);
        }

        public async Task<bool> IsUserRegisteredForEventAsync(string email, int eventId)
        {
            return await _dbSet
                .AnyAsync(ea => ea.Email == email && ea.EventId == eventId);
        }
    }
}
