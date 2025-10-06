namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class HomestayV1DTO
    {
        public long HomestayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LocationText { get; set; }
        public decimal? MinPrice { get; set; }
        public int? Status { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
