using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;
using LocalEventFinder.Repositories;

namespace LocalEventFinder.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepo;

        public VenueService(IVenueRepository vr)
        {
            _venueRepo = vr;
        }

        private static VenueDto MapVenueDTO(Venue venue)
        {
            return new VenueDto
            {
                Id = venue.Id,
                Name = venue.Name,
                Address = venue.Address,
                Capacity = venue.Capacity
            };
        }

        /// <summary>
        /// Создать новое место проведения
        /// </summary>
        public async Task<VenueDto> CreateAsync(CreateVenueDto createVenueDTO)
        {
            Venue newVenue = new Venue
            {
                Name = createVenueDTO.Name,
                Address = createVenueDTO.Address,
                Capacity = createVenueDTO.Capacity
            };

            var createdVenue = await _venueRepo.AddAsync(newVenue);
            return MapVenueDTO(createdVenue);
        }

        /// <summary>
        /// Удалить место проведения
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            return await _venueRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Получить все места проведения
        /// </summary>
        public async Task<IEnumerable<VenueDto>> GetAllVenuesAsync()
        {
            var venues = await _venueRepo.GetAllAsync();
            return venues.Select(MapVenueDTO);
        }

        /// <summary>
        /// Получить место проведения по ID
        /// </summary>
        public async Task<VenueDto?> GetByIdAsync(int id)
        {
            var venue = await _venueRepo.GetByIdAsync(id);
            return venue == null ? null : MapVenueDTO(venue);
        }

        /// <summary>
        /// Получить места проведения по вместимости
        /// </summary>
        public async Task<IEnumerable<VenueDto>> GetVenuesByCapacityAsync(int minCapacity, int maxCapacity)
        {
            var venues = await _venueRepo.GetVenuesByCapacityAsync(minCapacity, maxCapacity);
            return venues.Select(MapVenueDTO);
        }

        /// <summary>
        /// Обновить место проведения
        /// </summary>
        public async Task<VenueDto?> UpdateAsync(int id, CreateVenueDto updateVenueDTO)
        {
            var venue = await _venueRepo.GetByIdAsync(id);
            if (venue == null) return null;

            venue.Name = updateVenueDTO.Name;
            venue.Address = updateVenueDTO.Address;
            venue.Capacity = updateVenueDTO.Capacity;

            var updatedVenue = await _venueRepo.UpdateAsync(venue);
            return MapVenueDTO(updatedVenue);
        }
    }
}
