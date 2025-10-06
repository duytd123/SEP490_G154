using SEP490_G154_Service.DTOs.MaLogin;

namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class HomestayDetailDTO
    {
        public long HomestayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LocationText { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<string> Images { get; set; } = new();
        public List<RoomTypeDetailDTO> RoomTypes { get; set; } = new();
    }
}
