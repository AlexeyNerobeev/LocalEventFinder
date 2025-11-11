namespace LocalEventFinder.Models.DTO
{
    /// <summary>
    /// DTO для обновления места проведения
    /// </summary>
    public class UpdateVenueDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
