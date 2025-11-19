using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalEventFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizersController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;
        private readonly ILogger<OrganizersController> _logger;

        public OrganizersController(IOrganizerService organizerService, ILogger<OrganizersController> logger)
        {
            _organizerService = organizerService;
            _logger = logger;
        }

        /// <summary>
        /// Получить всех организаторов
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrganizers()
        {
            try
            {
                var organizers = await _organizerService.GetAllOrganizersAsync();
                return Ok(new
                {
                    success = true,
                    data = organizers,
                    count = organizers.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка организаторов");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении организаторов" }
                });
            }
        }

        /// <summary>
        /// Получить организатора по ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrganizer(int id)
        {
            try
            {
                var organizerDto = await _organizerService.GetByIdAsync(id);
                if (organizerDto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = new { message = $"Организатор с ID {id} не найден." }
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = organizerDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении организатора с ID {OrganizerId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении организатора" }
                });
            }
        }

        /// <summary>
        /// Получить организаторов с событиями
        /// </summary>
        [HttpGet("with-events")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrganizersWithEvents()
        {
            try
            {
                var organizers = await _organizerService.GetOrganizersWithEventsAsync();
                return Ok(new
                {
                    success = true,
                    data = organizers,
                    count = organizers.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении организаторов с событиями");
                return StatusCode(500, new
                {
                    success = false,
                    error = new { message = "Ошибка при получении организаторов с событиями" }
                });
            }
        }

        /// <summary>
        /// Создать нового организатора
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrganizerDto>> CreateOrganizer(CreateOrganizerDto createOrganizerDto)
        {
            try
            {
                var organizerDto = await _organizerService.CreateAsync(createOrganizerDto);
                return CreatedAtAction(nameof(GetOrganizer), new { id = organizerDto.Id }, organizerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании организатора");
                throw;
            }
        }

        /// <summary>
        /// Обновить организатора
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<OrganizerDto>> UpdateOrganizer(int id, CreateOrganizerDto updateOrganizerDto)
        {
            try
            {
                var organizerDto = await _organizerService.UpdateAsync(id, updateOrganizerDto);
                if (organizerDto == null)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Организатор с ID {id} не найден.",
                        instance = $"/api/organizers/{id}"
                    });
                }

                return Ok(organizerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении организатора с ID {OrganizerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Удалить организатора
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganizer(int id)
        {
            try
            {
                var result = await _organizerService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        title = "Not Found",
                        status = 404,
                        detail = $"Организатор с ID {id} не найден.",
                        instance = $"/api/organizers/{id}"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении организатора с ID {OrganizerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Получить организаторов по домену email
        /// </summary>
        [HttpGet("email-domain/{emailDomain}")]
        public async Task<ActionResult<IEnumerable<OrganizerDto>>> GetOrganizersByEmailDomain(string emailDomain)
        {
            try
            {
                var organizers = await _organizerService.GetOrganizersByEmailDomainAsync(emailDomain);
                return Ok(organizers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении организаторов по домену email {EmailDomain}", emailDomain);
                throw;
            }
        }
    }
}
