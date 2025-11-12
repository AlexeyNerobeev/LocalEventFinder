using LocalEventFinder.Models.DTO;

namespace LocalEventFinder.Services
{
    /// <summary>
    /// Сервис для работы с участниками мероприятий
    /// </summary>
    public interface IEventAttendeeService
    {
        /// <summary>
        /// Получить всех участников
        /// </summary>
        Task<IEnumerable<EventAttendeeDto>> GetAllAttendeesAsync();

        /// <summary>
        /// Получить участника по ID
        /// </summary>
        Task<EventAttendeeDto?> GetByIdAsync(int id);

        /// <summary>
        /// Зарегистрировать участника на мероприятие
        /// </summary>
        Task<EventAttendeeDto> RegisterForEventAsync(int eventId, RegisterForEventDto registerDto);

        /// <summary>
        /// Отменить регистрацию участника
        /// </summary>
        Task<bool> CancelRegistrationAsync(int id);

        /// <summary>
        /// Получить участников по мероприятию
        /// </summary>
        Task<IEnumerable<EventAttendeeDto>> GetAttendeesByEventAsync(int eventId);

        /// <summary>
        /// Получить участников по email
        /// </summary>
        Task<IEnumerable<EventAttendeeDto>> GetAttendeesByEmailAsync(string email);
    }
}
