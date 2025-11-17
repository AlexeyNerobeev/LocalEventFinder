using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalEventFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto registerDto)
        {
            try
            {
                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(registerDto.Username) ||
                    string.IsNullOrWhiteSpace(registerDto.Email) ||
                    string.IsNullOrWhiteSpace(registerDto.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = new
                        {
                            message = "Все поля обязательны для заполнения",
                            code = "VALIDATION_ERROR"
                        }
                    });
                }

                if (registerDto.Password.Length < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = new
                        {
                            message = "Пароль должен содержать минимум 6 символов",
                            code = "PASSWORD_TOO_SHORT"
                        }
                    });
                }

                var result = await _authService.RegisterAsync(registerDto);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Ошибка регистрации: {Message}", ex.Message);
                return BadRequest(new
                {
                    success = false,
                    error = new
                    {
                        message = ex.Message,
                        code = "USER_EXISTS"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя");
                return StatusCode(500, new
                {
                    success = false,
                    error = new
                    {
                        message = "Внутренняя ошибка сервера",
                        code = "INTERNAL_ERROR"
                    }
                });
            }
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Email) ||
                    string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = new
                        {
                            message = "Email и пароль обязательны для заполнения",
                            code = "VALIDATION_ERROR"
                        }
                    });
                }

                var result = await _authService.LoginAsync(loginDto);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неудачная попытка входа для email: {Email}", loginDto.Email);
                return Unauthorized(new
                {
                    success = false,
                    error = new
                    {
                        message = ex.Message,
                        code = "INVALID_CREDENTIALS"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя");
                return StatusCode(500, new
                {
                    success = false,
                    error = new
                    {
                        message = "Внутренняя ошибка сервера",
                        code = "INTERNAL_ERROR"
                    }
                });
            }
        }

        /// <summary>
        /// Проверка токена (для тестирования)
        /// </summary>
        [HttpGet("verify")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Verify()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                success = true,
                data = new
                {
                    message = "Токен валиден",
                    userId,
                    username,
                    email,
                    role
                }
            });
        }
    }
}
