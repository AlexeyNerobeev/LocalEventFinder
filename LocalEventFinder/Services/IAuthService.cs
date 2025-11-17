using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;

namespace LocalEventFinder.Services
{
    /// <summary>
    /// Сервис для аутентификации и управления пользователями
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto);

        /// <summary>
        /// Вход пользователя
        /// </summary>
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto);

        /// <summary>
        /// Проверка существования пользователя по email
        /// </summary>
        Task<bool> UserExistsAsync(string email);

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);
    }
}
