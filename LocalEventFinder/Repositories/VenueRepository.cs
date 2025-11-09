using LocalEventFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalEventFinder.Repositories
{
    public class VenueRepository : Repository<Venue>, IVenueRepository
    {
        public VenueRepository(EventDbContext context) : base(context) { }

        public async Task<IEnumerable<Venue>> GetVenuesByCapacityAsync(int minCapacity, int maxCapacity)
        {
            return await _dbSet
                .Where(v => v.Capacity >= minCapacity && v.Capacity <= maxCapacity)
                .OrderBy(v => v.Capacity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venue>> GetVenuesWithEventsAsync()
        {
            return await _dbSet
                .Include(v => v.Events)
                .Where(v => v.Events.Any())
                .ToListAsync();
        }

        public async Task<Venue?> GetVenueWithEventsAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
