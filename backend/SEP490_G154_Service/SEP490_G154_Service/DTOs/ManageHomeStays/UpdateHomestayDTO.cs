namespace SEP490_G154_Service.DTOs.ManageHomeStays
{
    public class UpdateHomestayDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }       // Mô tả homestay
        public string? LocationText { get; set; }      // Địa chỉ hoặc khu vực
        public int Status { get; set; } = 1;           // 1: hoạt động, 0: ẩn/tạm dừng
        public List<string>? ImageUrls { get; set; }
    }
}
