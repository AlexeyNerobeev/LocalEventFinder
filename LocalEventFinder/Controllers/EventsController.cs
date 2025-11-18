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
        /// Получить все мероприятия
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мероприятий");
                throw;
            }
        }

        /// <summary>
        /// Получить мероприятие по ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Organizer,User")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            try
            {
                var eventDto = await _eventService.GetByIdAsync(id);
                if (eventDto == null)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Мероприятие с ID {id} не найдено.",
                        instance = $"/api/events/{id}"
                    });
                }

                return Ok(eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мероприятия с ID {EventId}", id);
                throw;
            }
        }

        /// <summary>
        /// Создать новое мероприятие
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto createEventDto)
        {
            try
            {
                var eventDto = await _eventService.CreateAsync(createEventDto);
                return CreatedAtAction(nameof(GetEvent), new { id = eventDto.Id }, eventDto);
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
                _logger.LogError(ex, "Ошибка при создании мероприятия");
                throw;
            }
        }

        /// <summary>
        /// Обновить мероприятие
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<EventDto>> UpdateEvent(int id, CreateEventDto updateEventDto)
        {
            try
            {
                var eventDto = await _eventService.UpdateAsync(id, updateEventDto);
                if (eventDto == null)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Мероприятие с ID {id} не найдено.",
                        instance = $"/api/events/{id}"
                    });
                }

                return Ok(eventDto);
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
                _logger.LogError(ex, "Ошибка при обновлении мероприятия с ID {EventId}", id);
                throw;
            }
        }

        /// <summary>
        /// Удалить мероприятие
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _eventService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Мероприятие с ID {id} не найдено.",
                        instance = $"/api/events/{id}"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении мероприятия с ID {EventId}", id);
                throw;
            }
        }

        /// <summary>
        /// Получить предстоящие мероприятия
        /// </summary>
        [HttpGet("upcoming")]
        [Authorize(Roles = "Admin,Organizer,User")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetUpcomingEvents([FromQuery] int days = 30)
        {
            try
            {
                var events = await _eventService.GetUpcomingEventsAsync(days);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении предстоящих мероприятий");
                throw;
            }
        }

        /// <summary>
        /// Получить мероприятия по категории
        /// </summary>
        [HttpGet("category/{category}")]
        [Authorize(Roles = "Admin,Organizer,User")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByCategory(string category)
        {
            try
            {
                var events = await _eventService.GetEventsByCategoryAsync(category);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мероприятий по категории {Category}", category);
                throw;
            }
        }
    }
}
