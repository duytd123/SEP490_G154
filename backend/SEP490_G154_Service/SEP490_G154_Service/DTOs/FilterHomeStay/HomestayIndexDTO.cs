namespace SEP490_G154_Service.DTOs.FilterHomeStay
{
    public class HomestayIndexDTO
    {
        public long HomestayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LocationText { get; set; }
        public List<string> RoomTypeNames { get; set; } = new();
    }
}
