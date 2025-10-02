namespace SEP490_G154_Service.DTOs.FilterHomeStay
{
    public class FilterHeaderRequestDTO
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Guests { get; set; }
        public int Rooms { get; set; }
    }
}
