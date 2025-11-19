using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalEventFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все мероприятия (публичный доступ)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(new
                {
                    success = true,
                    data = events,
                    count = events.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении мероприятий" }
                });
            }
        }

        /// <summary>
        /// Получить мероприятие по ID (публичный доступ)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetEvent(int id)
        {
            try
            {
                var eventDto = await _eventService.GetByIdAsync(id);
                if (eventDto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Мероприятие с ID {id} не найдено." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = eventDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мероприятия с ID {EventId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении мероприятия" }
                });
            }
        }

        /// <summary>
        /// Создать новое мероприятие (только организаторы и администраторы)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            try
            {
                var eventDto = await _eventService.CreateAsync(createEventDto);
                return Ok(new
                {
                    success = true,
                    data = eventDto,
                    message = "Мероприятие успешно создано"
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
                _logger.LogError(ex, "Ошибка при создании мероприятия");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при создании мероприятия" }
                });
            }
        }

        /// <summary>
        /// Обновить мероприятие (только организаторы и администраторы)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "OrganizerOnly")]
        public async Task<ActionResult> UpdateEvent(int id, [FromBody] CreateEventDto updateEventDto)
        {
            try
            {
                var eventDto = await _eventService.UpdateAsync(id, updateEventDto);
                if (eventDto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Мероприятие с ID {id} не найдено." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = eventDto,
                    message = "Мероприятие успешно обновлено"
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
                _logger.LogError(ex, "Ошибка при обновлении мероприятия с ID {EventId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при обновлении мероприятия" }
                });
            }
        }

        /// <summary>
        /// Удалить мероприятие (только администраторы)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _eventService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Мероприятие с ID {id} не найдено." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Мероприятие успешно удалено"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении мероприятия с ID {EventId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при удалении мероприятия" }
                });
            }
        }

        /// <summary>
        /// Получить предстоящие мероприятия (публичный доступ)
        /// </summary>
        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult> GetUpcomingEvents([FromQuery] int days = 30)
        {
            try
            {
                var events = await _eventService.GetUpcomingEventsAsync(days);
                return Ok(new
                {
                    success = true,
                    data = events,
                    count = events.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении предстоящих мероприятий");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении предстоящих мероприятий" }
                });
            }
        }
    }
}
