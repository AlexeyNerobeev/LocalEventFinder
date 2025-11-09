using LocalEventFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalEventFinder.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(EventDbContext context) : base(context) { }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                .OrderBy(e => e.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(e => e.Category == category)
                .OrderBy(e => e.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 30)
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            return await GetEventsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Event>> GetEventsWithDetailsAsync()
        {
            return await _dbSet
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .Include(e => e.Attendees)
                .OrderBy(e => e.DateTime)
                .ToListAsync();
        }

        public async Task<Event?> GetEventWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(int organizerId)
        {
            return await _dbSet
                .Where(e => e.OrganizerId == organizerId)
                .OrderBy(e => e.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByVenueAsync(int venueId)
        {
            return await _dbSet
                .Where(e => e.VenueId == venueId)
                .OrderBy(e => e.DateTime)
                .ToListAsync();
        }

        public async Task<bool> IsVenueAvailableAsync(int venueId, DateTime dateTime, int duration)
        {
            var endTime = dateTime.AddMinutes(duration);

            return !await _dbSet
                .AnyAsync(e => e.VenueId == venueId &&
                              e.DateTime < endTime &&
                              e.DateTime.AddMinutes(e.Duration) > dateTime);
        }
    }
}
