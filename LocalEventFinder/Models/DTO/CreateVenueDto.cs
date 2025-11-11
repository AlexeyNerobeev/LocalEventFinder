namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для создания нового места проведения
    /// </summary>
    public class CreateVenueDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
