using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;
using LocalEventFinder.Repositories;

namespace LocalEventFinder.Services
{
    public class EventAttendeeService : IEventAttendeeService
    {
        private readonly IEventAttendeeRepository _attendeeRepo;
        private readonly IEventRepository _eventRepo;

        public EventAttendeeService(IEventAttendeeRepository ear, IEventRepository er)
        {
            _attendeeRepo = ear;
            _eventRepo = er;
        }

        private static EventAttendeeDto MapAttendeeDTO(EventAttendee attendee)
        {
            return new EventAttendeeDto
            {
                Id = attendee.Id,
                UserName = attendee.UserName,
                Email = attendee.Email,
                RegistrationDate = attendee.RegistrationDate,
                EventId = attendee.EventId
            };
        }

        /// <summary>
        /// Отменить регистрацию участника
        /// </summary>
        public async Task<bool> CancelRegistrationAsync(int id)
        {
            return await _attendeeRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Получить всех участников
        /// </summary>
        public async Task<IEnumerable<EventAttendeeDto>> GetAllAttendeesAsync()
        {
            var attendees = await _attendeeRepo.GetAllAsync();
            return attendees.Select(MapAttendeeDTO);
        }

        /// <summary>
        /// Получить участников по email
        /// </summary>
        public async Task<IEnumerable<EventAttendeeDto>> GetAttendeesByEmailAsync(string email)
        {
            var attendees = await _attendeeRepo.GetAttendeesByEmailAsync(email);
            return attendees.Select(MapAttendeeDTO);
        }

        /// <summary>
        /// Получить участников по мероприятию
        /// </summary>
        public async Task<IEnumerable<EventAttendeeDto>> GetAttendeesByEventAsync(int eventId)
        {
            var attendees = await _attendeeRepo.GetAttendeesByEventAsync(eventId);
            return attendees.Select(MapAttendeeDTO);
        }

        /// <summary>
        /// Получить участника по ID
        /// </summary>
        public async Task<EventAttendeeDto?> GetByIdAsync(int id)
        {
            var attendee = await _attendeeRepo.GetByIdAsync(id);
            return attendee == null ? null : MapAttendeeDTO(attendee);
        }

        /// <summary>
        /// Зарегистрировать участника на мероприятие
        /// </summary>
        public async Task<EventAttendeeDto> RegisterForEventAsync(int eventId, RegisterForEventDto registerDto)
        {
            var existingEvent = await _eventRepo.GetByIdAsync(eventId);
            if (existingEvent == null)
                throw new ArgumentException("Мероприятие не найдено");

            bool isAlreadyRegistered = await _attendeeRepo.IsUserRegisteredForEventAsync(registerDto.Email, eventId);
            if (isAlreadyRegistered)
                throw new ArgumentException("Пользователь уже зарегистрирован на это мероприятие");

            int currentAttendees = await _attendeeRepo.GetAttendeesCountByEventAsync(eventId);
            if (currentAttendees >= existingEvent.MaxAttendees)
                throw new ArgumentException("Достигнуто максимальное количество участников");

            EventAttendee newAttendee = new EventAttendee
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                RegistrationDate = DateTime.UtcNow, 
                EventId = eventId
            };

            var createdAttendee = await _attendeeRepo.AddAsync(newAttendee);

            var attendeeWithEvent = await _attendeeRepo.FindAsync(ea => ea.Id == createdAttendee.Id);
            return MapAttendeeDTO(attendeeWithEvent.First());
        }
    }
}
