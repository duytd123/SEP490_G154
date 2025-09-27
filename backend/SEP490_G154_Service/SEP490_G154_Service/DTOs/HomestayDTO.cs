namespace SEP490_G154_Service.DTOs
{
    public class HomestayDTO
    {
        public long HomestayId { get; set; }
        public string Name { get; set; }
        public string? LocationText { get; set; }
        public List<RoomTypeDTO> RoomTypes { get; set; } = new List<RoomTypeDTO>();
    }
}
