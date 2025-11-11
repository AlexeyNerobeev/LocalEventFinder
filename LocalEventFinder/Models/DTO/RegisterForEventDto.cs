namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для регистрации на событие
    /// </summary>
    public class RegisterForEventDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
