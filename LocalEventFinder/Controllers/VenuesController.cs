using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalEventFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        private readonly ILogger<VenuesController> _logger;

        public VenuesController(IVenueService venueService, ILogger<VenuesController> logger)
        {
            _venueService = venueService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все места проведения
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetVenues()
        {
            try
            {
                var venues = await _venueService.GetAllVenuesAsync();
                return Ok(new
                {
                    success = true,
                    data = venues,
                    count = venues.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мест проведения");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении мест проведения" }
                });
            }
        }

        /// <summary>
        /// Получить место проведения по ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVenue(int id)
        {
            try
            {
                var venueDto = await _venueService.GetByIdAsync(id);
                if (venueDto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Место проведения с ID {id} не найдено." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = venueDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении места проведения с ID {VenueId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении места проведения" }
                });
            }
        }

        /// <summary>
        /// Получить места проведения с событиями
        /// </summary>
        [HttpGet("with-events")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVenuesWithEvents()
        {
            try
            {
                var venues = await _venueService.GetVenuesWithEventsAsync();
                return Ok(new
                {
                    success = true,
                    data = venues,
                    count = venues.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мест проведения с событиями");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении мест проведения с событиями" }
                });
            }
        }

        /// <summary>
        /// Получить статистику по местам проведения
        /// </summary>
        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVenuesStats()
        {
            try
            {
                var stats = await _venueService.GetVenuesStatsAsync();
                return Ok(new
                {
                    success = true,
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики мест проведения");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении статистики" }
                });
            }
        }

        /// <summary>
        /// Получить места проведения по вместимости
        /// </summary>
        [HttpGet("capacity")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVenuesByCapacity(
            [FromQuery] int minCapacity, [FromQuery] int maxCapacity)
        {
            try
            {
                var venues = await _venueService.GetVenuesByCapacityAsync(minCapacity, maxCapacity);
                return Ok(new
                {
                    success = true,
                    data = venues,
                    count = venues.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мест проведения по вместимости");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении мест проведения по вместимости" }
                });
            }
        }

        /// <summary>
        /// Создать новое место проведения (только администраторы)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> CreateVenue([FromBody] CreateVenueDto createVenueDto)
        {
            try
            {
                var venueDto = await _venueService.CreateAsync(createVenueDto);
                return Ok(new
                {
                    success = true,
                    data = venueDto,
                    message = "Место проведения успешно создано"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании места проведения");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при создании места проведения" }
                });
            }
        }

        /// <summary>
        /// Обновить место проведения (только администраторы)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> UpdateVenue(int id, [FromBody] CreateVenueDto updateVenueDto)
        {
            try
            {
                var venueDto = await _venueService.UpdateAsync(id, updateVenueDto);
                if (venueDto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Место проведения с ID {id} не найдено." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = venueDto,
                    message = "Место проведения успешно обновлено"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении места проведения с ID {VenueId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при обновлении места проведения" }
                });
            }
        }

        /// <summary>
        /// Удалить место проведения (только администраторы)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteVenue(int id)
        {
            try
            {
                var result = await _venueService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Место проведения с ID {id} не найдено." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Место проведения успешно удалено"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении места проведения с ID {VenueId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при удалении места проведения" }
                });
            }
        }
    }
}
