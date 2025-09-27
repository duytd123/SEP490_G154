namespace SEP490_G154_Service.DTOs
{
    public class RoomTypeDTO
    {
        public long RoomTypeId { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int TotalRooms { get; set; }
        public decimal BasePrice { get; set; }
        public int AvailableRooms { get; set; }
    }
}
