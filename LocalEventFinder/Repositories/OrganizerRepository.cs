using LocalEventFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalEventFinder.Repositories
{
    public class OrganizerRepository : Repository<Organizer>, IOrganizerRepository
    {
        public OrganizerRepository(EventDbContext context) : base(context) { }

        public async Task<IEnumerable<Organizer>> GetOrganizersByEmailAsync(string emailDomain)
        {
            return await _dbSet
                .Where(o => o.Email.Contains(emailDomain))
                .ToListAsync();
        }

        public async Task<IEnumerable<Organizer>> GetOrganizersWithEventsAsync()
        {
            return await _dbSet
                .Include(o => o.Events)
                .Where(o => o.Events.Any())
                .ToListAsync();
        }

        public async Task<Organizer?> GetOrganizerWithEventsAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Events)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Organizer>> GetAllWithEventsAsync()
        {
            return await _dbSet
                .Include(o => o.Events)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Organizer>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.Events)
                .ToListAsync();
        }

        public override async Task<Organizer?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Events)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
