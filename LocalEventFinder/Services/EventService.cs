using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;
using LocalEventFinder.Repositories;

namespace LocalEventFinder.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IVenueRepository _venueRepo;
        private readonly IOrganizerRepository _organizerRepo;
        private readonly IEventAttendeeRepository _attendeeRepo;

        public EventService(IEventRepository er, IVenueRepository vr, IOrganizerRepository or, IEventAttendeeRepository ear)
        {
            _eventRepo = er;
            _venueRepo = vr;
            _organizerRepo = or;
            _attendeeRepo = ear;
        }

        private async Task<EventDto> MapEventDTO(Event eventEntity)
        {
            var currentAttendees = await _attendeeRepo.GetAttendeesCountByEventAsync(eventEntity.Id);

            return new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                DateTime = eventEntity.DateTime,
                Duration = eventEntity.Duration,
                Category = eventEntity.Category,
                Price = eventEntity.Price,
                MaxAttendees = eventEntity.MaxAttendees,
                CurrentAttendees = currentAttendees, 
                VenueId = eventEntity.VenueId,
                OrganizerId = eventEntity.OrganizerId
            };
        }

        /// <summary>
        /// Создать новое мероприятие
        /// </summary>
        public async Task<EventDto> CreateAsync(CreateEventDto createEventDTO)
        {
            var existingVenue = await _venueRepo.GetByIdAsync(createEventDTO.VenueId);
            if (existingVenue == null)
                throw new ArgumentException("Место проведения не найдено");

            var existingOrganizer = await _organizerRepo.GetByIdAsync(createEventDTO.OrganizerId);
            if (existingOrganizer == null)
                throw new ArgumentException("Организатор не найден");

            bool isVenueAvailable = await _eventRepo.IsVenueAvailableAsync(
                createEventDTO.VenueId,
                createEventDTO.DateTime,
                createEventDTO.Duration);

            if (!isVenueAvailable)
                throw new ArgumentException("Место проведения занято в указанное время");

            Event newEvent = new Event
            {
                Title = createEventDTO.Title,
                Description = createEventDTO.Description,
                DateTime = createEventDTO.DateTime,
                Duration = createEventDTO.Duration,
                Category = createEventDTO.Category,
                Price = createEventDTO.Price,
                MaxAttendees = createEventDTO.MaxAttendees,
                VenueId = createEventDTO.VenueId,
                OrganizerId = createEventDTO.OrganizerId
            };

            var createdEvent = await _eventRepo.AddAsync(newEvent);
            return await MapEventDTO(createdEvent);
        }

        /// <summary>
        /// Удалить мероприятие
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            return await _eventRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Получить все мероприятия
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _eventRepo.GetAllAsync();
            var eventDtos = new List<EventDto>();

            foreach (var eventEntity in events)
            {
                eventDtos.Add(await MapEventDTO(eventEntity));
            }

            return eventDtos;
        }

        /// <summary>
        /// Получить мероприятие по ID
        /// </summary>
        public async Task<EventDto?> GetByIdAsync(int id)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(id);
            return eventEntity == null ? null : await MapEventDTO(eventEntity);
        }

        /// <summary>
        /// Получить мероприятия по категории
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category)
        {
            var events = await _eventRepo.GetEventsByCategoryAsync(category);
            var eventDtos = new List<EventDto>();

            foreach (var eventEntity in events)
            {
                eventDtos.Add(await MapEventDTO(eventEntity));
            }

            return eventDtos;
        }

        /// <summary>
        /// Получить предстоящие мероприятия
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(int days = 30)
        {
            var events = await _eventRepo.GetUpcomingEventsAsync(days);
            var eventDtos = new List<EventDto>();

            foreach (var eventEntity in events)
            {
                eventDtos.Add(await MapEventDTO(eventEntity));
            }

            return eventDtos;
        }

        /// <summary>
        /// Обновить мероприятие
        /// </summary>
        public async Task<EventDto?> UpdateAsync(int id, CreateEventDto updateEventDTO)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(id);
            if (eventEntity == null) return null;

            var venueExists = await _venueRepo.ExistsAsync(updateEventDTO.VenueId);
            if (!venueExists)
                throw new ArgumentException("Место проведения с таким ID не найдено");

            var organizerExists = await _organizerRepo.ExistsAsync(updateEventDTO.OrganizerId);
            if (!organizerExists)
                throw new ArgumentException("Организатор с таким ID не найден");

            var conflictingEvents = await _eventRepo.FindAsync(e =>
                e.VenueId == updateEventDTO.VenueId &&
                e.Id != id &&
                e.DateTime < updateEventDTO.DateTime.AddMinutes(updateEventDTO.Duration) &&
                e.DateTime.AddMinutes(e.Duration) > updateEventDTO.DateTime);

            if (conflictingEvents.Any())
                throw new ArgumentException("Место проведения занято в указанное время");

            eventEntity.Title = updateEventDTO.Title;
            eventEntity.Description = updateEventDTO.Description;
            eventEntity.DateTime = updateEventDTO.DateTime;
            eventEntity.Duration = updateEventDTO.Duration;
            eventEntity.Category = updateEventDTO.Category;
            eventEntity.Price = updateEventDTO.Price;
            eventEntity.MaxAttendees = updateEventDTO.MaxAttendees;
            eventEntity.VenueId = updateEventDTO.VenueId;
            eventEntity.OrganizerId = updateEventDTO.OrganizerId;

            var updatedEvent = await _eventRepo.UpdateAsync(eventEntity);
            return await MapEventDTO(updatedEvent);
        }
    }
}
