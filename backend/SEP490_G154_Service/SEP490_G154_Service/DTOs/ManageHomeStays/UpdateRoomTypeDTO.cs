namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class UpdateRoomTypeDTO
    {
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public int TotalRooms { get; set; }
        public int? Capacity { get; set; }
        public int Status { get; set; }

    }
}
