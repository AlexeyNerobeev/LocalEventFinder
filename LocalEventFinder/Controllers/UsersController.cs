using LocalEventFinder.Models;
using LocalEventFinder.Repositories;
using LocalEventFinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LocalEventFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthService authService, ILogger<UsersController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Получить профиль текущего пользователя (для всех аутентифицированных)
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = "Пользователь не найден" }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role,
                        user.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля пользователя");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении профиля" }
                });
            }
        }

        /// <summary>
        /// Получить список всех пользователей (только для администраторов)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                // Для простоты используем репозиторий напрямую
                var userRepository = HttpContext.RequestServices.GetRequiredService<IRepository<User>>();
                var users = await userRepository.GetAllAsync();

                var userDtos = users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                });

                return Ok(new
                {
                    success = true,
                    data = userDtos,
                    count = users.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка пользователей");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении списка пользователей" }
                });
            }
        }

        /// <summary>
        /// Обновить роль пользователя (только для администраторов)
        /// </summary>
        [HttpPut("{id}/role")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {
                var userRepository = HttpContext.RequestServices.GetRequiredService<IRepository<User>>();
                var user = await userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Пользователь с ID {id} не найден" }
                    });
                }

                // Проверяем валидность роли
                var validRoles = new[] { "Admin", "Organizer", "User" };
                if (!validRoles.Contains(updateRoleDto.Role))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = new { message = "Недопустимая роль. Допустимые значения: Admin, Organizer, User" }
                    });
                }

                user.Role = updateRoleDto.Role;
                user.UpdatedAt = DateTime.UtcNow;

                await userRepository.UpdateAsync(user);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role
                    },
                    message = "Роль пользователя успешно обновлена"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении роли пользователя {UserId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при обновлении роли пользователя" }
                });
            }
        }
    }

    /// <summary>
    /// DTO для обновления роли пользователя
    /// </summary>
    public class UpdateRoleDto
    {
        public string Role { get; set; } = string.Empty;
    }
}
