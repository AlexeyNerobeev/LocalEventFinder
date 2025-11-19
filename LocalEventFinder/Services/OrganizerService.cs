using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;
using LocalEventFinder.Repositories;

namespace LocalEventFinder.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRepository _organizerRepo;

        public OrganizerService(IOrganizerRepository or)
        {
            _organizerRepo = or;
        }

        private static OrganizerDto MapOrganizerDTO(Organizer organizer)
        {
            return new OrganizerDto
            {
                Id = organizer.Id,
                Name = organizer.Name,
                ContactPhone = organizer.ContactPhone,
                Email = organizer.Email,
                EventsCount = organizer.Events?.Count ?? 0 
            };
        }

        /// <summary>
        /// Создать нового организатора
        /// </summary>
        public async Task<OrganizerDto> CreateAsync(CreateOrganizerDto createOrganizerDTO)
        {
            Organizer newOrganizer = new Organizer
            {
                Name = createOrganizerDTO.Name,
                ContactPhone = createOrganizerDTO.ContactPhone,
                Email = createOrganizerDTO.Email
            };

            var createdOrganizer = await _organizerRepo.AddAsync(newOrganizer);
            return MapOrganizerDTO(createdOrganizer);
        }

        /// <summary>
        /// Удалить организатора
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            return await _organizerRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Получить всех организаторов
        /// </summary>
        public async Task<IEnumerable<OrganizerDto>> GetAllOrganizersAsync()
        {
            var organizers = await _organizerRepo.GetAllWithEventsAsync();
            return organizers.Select(MapOrganizerDTO);
        }

        /// <summary>
        /// Получить организатора по ID
        /// </summary>
        public async Task<OrganizerDto?> GetByIdAsync(int id)
        {
            var organizer = await _organizerRepo.GetByIdAsync(id);
            return organizer == null ? null : MapOrganizerDTO(organizer);
        }

        /// <summary>
        /// Получить организаторов по домену email
        /// </summary>
        public async Task<IEnumerable<OrganizerDto>> GetOrganizersByEmailDomainAsync(string emailDomain)
        {
            var organizers = await _organizerRepo.GetOrganizersByEmailAsync(emailDomain);
            return organizers.Select(MapOrganizerDTO);
        }

        /// <summary>
        /// Обновить организатора
        /// </summary>
        public async Task<OrganizerDto?> UpdateAsync(int id, CreateOrganizerDto updateOrganizerDTO)
        {
            var organizer = await _organizerRepo.GetByIdAsync(id);
            if (organizer == null) return null;

            organizer.Name = updateOrganizerDTO.Name;
            organizer.ContactPhone = updateOrganizerDTO.ContactPhone;
            organizer.Email = updateOrganizerDTO.Email;

            var updatedOrganizer = await _organizerRepo.UpdateAsync(organizer);

            var organizerWithEvents = await _organizerRepo.GetByIdAsync(id);
            return organizerWithEvents == null ? null : MapOrganizerDTO(organizerWithEvents);
        }

        /// <summary>
        /// Получить организаторов с событиями
        /// </summary>
        public async Task<IEnumerable<OrganizerDto>> GetOrganizersWithEventsAsync()
        {
            var organizers = await _organizerRepo.GetOrganizersWithEventsAsync();
            return organizers.Select(MapOrganizerDTO);
        }
    }
}
