using LocalEventFinder.Models;

namespace LocalEventFinder.Repositories
{
    public interface IOrganizerRepository : IRepository<Organizer>
    {
        Task<IEnumerable<Organizer>> GetOrganizersByEmailAsync(string emailDomain);
        Task<IEnumerable<Organizer>> GetOrganizersWithEventsAsync();
        Task<Organizer?> GetOrganizerWithEventsAsync(int id);
    }
}
