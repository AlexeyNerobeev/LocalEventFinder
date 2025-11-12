using LocalEventFinder.Models.DTO;

namespace LocalEventFinder.Services
{
    /// <summary>
    /// Сервис для работы с мероприятиями
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Получить все мероприятия
        /// </summary>
        Task<IEnumerable<EventDto>> GetAllEventsAsync();

        /// <summary>
        /// Получить мероприятие по ID
        /// </summary>
        Task<EventDto?> GetByIdAsync(int id);

        /// <summary>
        /// Создать новое мероприятие
        /// </summary>
        Task<EventDto> CreateAsync(CreateEventDto createEventDto);

        /// <summary>
        /// Обновить мероприятие
        /// </summary>
        Task<EventDto?> UpdateAsync(int id, CreateEventDto updateEventDto);

        /// <summary>
        /// Удалить мероприятие
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Получить предстоящие мероприятия
        /// </summary>
        Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(int days = 30);

        /// <summary>
        /// Получить мероприятия по категории
        /// </summary>
        Task<IEnumerable<EventDto>> GetEventsByCategoryAsync(string category);
    }
}
