namespace SEP490_G154_Service.DTOs.News
{
    public class NewsArticleDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
