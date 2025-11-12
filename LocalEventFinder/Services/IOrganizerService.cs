using LocalEventFinder.Models.DTO;

namespace LocalEventFinder.Services
{
    /// <summary>
    /// Сервис для работы с организаторами
    /// </summary>
    public interface IOrganizerService
    {
        /// <summary>
        /// Получить всех организаторов
        /// </summary>
        Task<IEnumerable<OrganizerDto>> GetAllOrganizersAsync();

        /// <summary>
        /// Получить организатора по ID
        /// </summary>
        Task<OrganizerDto?> GetByIdAsync(int id);

        /// <summary>
        /// Создать нового организатора
        /// </summary>
        Task<OrganizerDto> CreateAsync(CreateOrganizerDto createOrganizerDto);

        /// <summary>
        /// Обновить организатора
        /// </summary>
        Task<OrganizerDto?> UpdateAsync(int id, CreateOrganizerDto updateOrganizerDto);

        /// <summary>
        /// Удалить организатора
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Получить организаторов по домену email
        /// </summary>
        Task<IEnumerable<OrganizerDto>> GetOrganizersByEmailDomainAsync(string emailDomain);
    }
}
