namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class RoomTypeDetailDTO
    {
        public long RoomTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int TotalRooms { get; set; }
        public int? Capacity { get; set; }
        public int? Status { get; set; }
    }
}
