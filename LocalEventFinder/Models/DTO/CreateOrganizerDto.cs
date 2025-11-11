namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для создания нового организатора
    /// </summary>
    public class CreateOrganizerDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
