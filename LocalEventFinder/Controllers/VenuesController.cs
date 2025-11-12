using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
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
        public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues()
        {
            try
            {
                var venues = await _venueService.GetAllVenuesAsync();
                return Ok(venues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка мест проведения");
                throw;
            }
        }

        /// <summary>
        /// Получить место проведения по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VenueDto>> GetVenue(int id)
        {
            try
            {
                var venueDto = await _venueService.GetByIdAsync(id);
                if (venueDto == null)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Место проведения с ID {id} не найдено.",
                        instance = $"/api/venues/{id}"
                    });
                }

                return Ok(venueDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении места проведения с ID {VenueId}", id);
                throw;
            }
        }

        /// <summary>
        /// Создать новое место проведения
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VenueDto>> CreateVenue(CreateVenueDto createVenueDto)
        {
            try
            {
                var venueDto = await _venueService.CreateAsync(createVenueDto);
                return CreatedAtAction(nameof(GetVenue), new { id = venueDto.Id }, venueDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании места проведения");
                throw;
            }
        }

        /// <summary>
        /// Обновить место проведения
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<VenueDto>> UpdateVenue(int id, CreateVenueDto updateVenueDto)
        {
            try
            {
                var venueDto = await _venueService.UpdateAsync(id, updateVenueDto);
                if (venueDto == null)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Место проведения с ID {id} не найдено.",
                        instance = $"/api/venues/{id}"
                    });
                }

                return Ok(venueDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении места проведения с ID {VenueId}", id);
                throw;
            }
        }

        /// <summary>
        /// Удалить место проведения
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            try
            {
                var result = await _venueService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Место проведения с ID {id} не найдено.",
                        instance = $"/api/venues/{id}"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении места проведения с ID {VenueId}", id);
                throw;
            }
        }

        /// <summary>
        /// Получить места проведения по вместимости
        /// </summary>
        [HttpGet("capacity")]
        public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenuesByCapacity(
            [FromQuery] int minCapacity, [FromQuery] int maxCapacity)
        {
            try
            {
                var venues = await _venueService.GetVenuesByCapacityAsync(minCapacity, maxCapacity);
                return Ok(venues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мест проведения по вместимости");
                throw;
            }
        }
    }
}
