using LocalEventFinder.Models.DTO;

namespace LocalEventFinder.Services
{
    /// <summary>
    /// Сервис для работы с местами проведения
    /// </summary>
    public interface IVenueService
    {
        /// <summary>
        /// Получить все места проведения
        /// </summary>
        Task<IEnumerable<VenueDto>> GetAllVenuesAsync();

        /// <summary>
        /// Получить место проведения по ID
        /// </summary>
        Task<VenueDto?> GetByIdAsync(int id);

        /// <summary>
        /// Создать новое место проведения
        /// </summary>
        Task<VenueDto> CreateAsync(CreateVenueDto createVenueDto);

        /// <summary>
        /// Обновить место проведения
        /// </summary>
        Task<VenueDto?> UpdateAsync(int id, CreateVenueDto updateVenueDto);

        /// <summary>
        /// Удалить место проведения
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Получить места проведения по вместимости
        /// </summary>
        Task<IEnumerable<VenueDto>> GetVenuesByCapacityAsync(int minCapacity, int maxCapacity);
    }
}
