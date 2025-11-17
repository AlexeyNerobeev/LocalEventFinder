namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для запроса входа
    /// </summary>
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
