namespace SEP490_G154_Service.DTOs.FilterHomeStay
{
    public class FilterWithElastic
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Guests { get; set; }
        public int Rooms { get; set; }
        public double? MinPrice { get; set; }  
        public double? MaxPrice { get; set; }
        public string? Keyword { get; set; }
    }
}
