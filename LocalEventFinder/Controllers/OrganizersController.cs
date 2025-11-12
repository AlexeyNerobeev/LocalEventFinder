using LocalEventFinder.Models.DTO;
using LocalEventFinder.Services;
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
        public async Task<ActionResult<IEnumerable<OrganizerDto>>> GetOrganizers()
        {
            try
            {
                var organizers = await _organizerService.GetAllOrganizersAsync();
                return Ok(organizers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка организаторов");
                throw;
            }
        }

        /// <summary>
        /// Получить организатора по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizerDto>> GetOrganizer(int id)
        {
            try
            {
                var organizerDto = await _organizerService.GetByIdAsync(id);
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
                _logger.LogError(ex, "Ошибка при получении организатора с ID {OrganizerId}", id);
                throw;
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
