namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class CreateRoomTypeDTO
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public int TotalRooms { get; set; }
        public int? Capacity { get; set; }

    }
}
