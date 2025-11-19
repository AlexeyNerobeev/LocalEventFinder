using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalEventFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventAttendeesController : ControllerBase
    {
        private readonly IEventAttendeeService _attendeeService;
        private readonly ILogger<EventAttendeesController> _logger;

        public EventAttendeesController(IEventAttendeeService attendeeService, ILogger<EventAttendeesController> logger)
        {
            _attendeeService = attendeeService;
            _logger = logger;
        }

        /// <summary>
        /// Получить всех участников (только администраторы)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> GetAttendees()
        {
            try
            {
                var attendees = await _attendeeService.GetAllAttendeesAsync();
                return Ok(new
                {
                    success = true,
                    data = attendees,
                    count = attendees.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка участников");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении участников" }
                });
            }
        }

        /// <summary>
        /// Получить участника по ID (только администраторы)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> GetAttendee(int id)
        {
            try
            {
                var attendeeDto = await _attendeeService.GetByIdAsync(id);
                if (attendeeDto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Участник с ID {id} не найден." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = attendeeDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении участника с ID {AttendeeId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении участника" }
                });
            }
        }

        /// <summary>
        /// Зарегистрировать участника на мероприятие (любой аутентифицированный пользователь)
        /// </summary>
        [HttpPost("events/{eventId}/register")]
        [Authorize(Policy = "AnyRole")]
        public async Task<ActionResult> RegisterForEvent(int eventId, [FromBody] RegisterForEventDto registerDto)
        {
            try
            {
                var attendeeDto = await _attendeeService.RegisterForEventAsync(eventId, registerDto);
                return Ok(new
                {
                    success = true,
                    data = attendeeDto,
                    message = "Регистрация на мероприятие прошла успешно"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = new { message = ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации участника на мероприятие {EventId}", eventId);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при регистрации на мероприятие" }
                });
            }
        }

        /// <summary>
        /// Отменить регистрацию участника (только администраторы или сам пользователь)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AnyRole")]
        public async Task<ActionResult> CancelRegistration(int id)
        {
            try
            {
                // TODO: Добавить проверку, что пользователь отменяет свою регистрацию или является админом
                var result = await _attendeeService.CancelRegistrationAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Регистрация с ID {id} не найдена." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Регистрация успешно отменена"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отмене регистрации участника с ID {AttendeeId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при отмене регистрации" }
                });
            }
        }

        /// <summary>
        /// Получить участников по мероприятию (только организаторы и администраторы)
        /// </summary>
        [HttpGet("events/{eventId}")]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> GetAttendeesByEvent(int eventId)
        {
            try
            {
                var attendees = await _attendeeService.GetAttendeesByEventAsync(eventId);
                return Ok(new
                {
                    success = true,
                    data = attendees,
                    count = attendees.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении участников мероприятия {EventId}", eventId);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении участников мероприятия" }
                });
            }
        }
    }
}
