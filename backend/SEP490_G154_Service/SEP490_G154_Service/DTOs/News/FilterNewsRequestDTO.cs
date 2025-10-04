namespace SEP490_G154_Service.DTOs.News
{
    public class FilterNewsRequestDTO
    {
        public string? Keyword { get; set; } = "technology";
        public string? Source { get; set; } // ví dụ: "vnexpress.net"
        public int? Days { get; set; } = 3; // số ngày gần nhất
        public int? Limit { get; set; } = 10; // số bài muốn lấy
    }
}
