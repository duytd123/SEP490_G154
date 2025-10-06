namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class CreateHomestayDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LocationText { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
