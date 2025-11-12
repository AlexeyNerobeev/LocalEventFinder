using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
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
        /// Получить всех участников
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventAttendeeDto>>> GetAttendees()
        {
            try
            {
                var attendees = await _attendeeService.GetAllAttendeesAsync();
                return Ok(attendees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка участников");
                throw;
            }
        }

        /// <summary>
        /// Получить участника по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventAttendeeDto>> GetAttendee(int id)
        {
            try
            {
                var attendeeDto = await _attendeeService.GetByIdAsync(id);
                if (attendeeDto == null)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Участник с ID {id} не найден.",
                        instance = $"/api/eventattendees/{id}"
                    });
                }

                return Ok(attendeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении участника с ID {AttendeeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Зарегистрировать участника на мероприятие
        /// </summary>
        [HttpPost("events/{eventId}/register")]
        public async Task<ActionResult<EventAttendeeDto>> RegisterForEvent(int eventId, RegisterForEventDto registerDto)
        {
            try
            {
                var attendeeDto = await _attendeeService.RegisterForEventAsync(eventId, registerDto);
                return CreatedAtAction(nameof(GetAttendee), new { id = attendeeDto.Id }, attendeeDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    title = "Bad Request",
                    status = 400,
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации участника на мероприятие {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Отменить регистрацию участника
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelRegistration(int id)
        {
            try
            {
                var result = await _attendeeService.CancelRegistrationAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Участник с ID {id} не найден.",
                        instance = $"/api/eventattendees/{id}"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отмене регистрации участника с ID {AttendeeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Получить участников по мероприятию
        /// </summary>
        [HttpGet("events/{eventId}")]
        public async Task<ActionResult<IEnumerable<EventAttendeeDto>>> GetAttendeesByEvent(int eventId)
        {
            try
            {
                var attendees = await _attendeeService.GetAttendeesByEventAsync(eventId);
                return Ok(attendees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении участников мероприятия {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Получить участников по email
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<IEnumerable<EventAttendeeDto>>> GetAttendeesByEmail(string email)
        {
            try
            {
                var attendees = await _attendeeService.GetAttendeesByEmailAsync(email);
                return Ok(attendees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении участников по email {Email}", email);
                throw;
            }
        }
    }
}
