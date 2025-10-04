using SEP490_G154_Service.DTOs.News;

namespace SEP490_G154_Service.Interface
{
    public interface INews
    {
        Task<IEnumerable<NewsArticleDTO>> GetNewsAsync(string keyword);
        Task<object> FilterNewsAsync(FilterNewsRequestDTO request);
        void ClearCache();
    }
}
