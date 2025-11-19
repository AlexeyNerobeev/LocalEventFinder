using LocalEventFinder.Models;

namespace LocalEventFinder.Repositories
{
    public interface IVenueRepository : IRepository<Venue>
    {
        Task<IEnumerable<Venue>> GetVenuesByCapacityAsync(int minCapacity, int maxCapacity);
        Task<IEnumerable<Venue>> GetVenuesWithEventsAsync();
        Task<Venue?> GetVenueWithEventsAsync(int id);
        Task<IEnumerable<Venue>> GetAllWithEventsAsync();
    }
}
