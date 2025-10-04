using Newtonsoft.Json.Linq;
using SEP490_G154_Service.DTOs.News;
using SEP490_G154_Service.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace SEP490_G154_Service.Service
{
    public class NewsService : INews
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private static readonly List<string> _cacheKeys = new();
        public NewsService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;

            // Bắt buộc phải có User-Agent cho NewsAPI
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SEP490_G154_NewsClient/1.0");
        }

        public void ClearCache()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear();
       
        }

        public async Task<IEnumerable<NewsArticleDTO>> GetNewsAsync(string keyword)
        {
            // Nếu không nhập keyword → mặc định là Vietnam
            if (string.IsNullOrWhiteSpace(keyword))
                keyword = "Vietnam";

            string cacheKey = $"news_{keyword.ToLower()}";

            //  1️ Kiểm tra cache trước
            if (_cache.TryGetValue(cacheKey, out IEnumerable<NewsArticleDTO> cachedNews))
            {
                Console.WriteLine($" Lấy từ cache: {keyword}");
                return cachedNews;
            }

            //  2️ Gọi API nếu cache chưa có
            string apiKey = _configuration["NewsApi:ApiKey"];
            string url = $"https://newsapi.org/v2/everything?q={Uri.EscapeDataString(keyword)}&apiKey={apiKey}&language=vi&pageSize=10";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"News API failed: {response.StatusCode} - {errorBody}");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var articles = new List<NewsArticleDTO>();
            foreach (var article in json["articles"])
            {
                articles.Add(new NewsArticleDTO
                {
                    Title = article["title"]?.ToString(),
                    Description = article["description"]?.ToString(),
                    Url = article["url"]?.ToString(),
                    Source = article["source"]?["name"]?.ToString(),
                    PublishedAt = DateTime.TryParse(article["publishedAt"]?.ToString(), out DateTime date)
                        ? date
                        : DateTime.MinValue
                });
            }

            // 3️ Lưu cache trong 5 phút
            _cache.Set(cacheKey, articles, TimeSpan.FromMinutes(5));

            Console.WriteLine($"Đã lưu cache cho keyword: {keyword}");

            return articles;
        }

        public async Task<object> FilterNewsAsync(FilterNewsRequestDTO request)
        {
            var cacheKey = $"news_{request.Keyword}_{request.Source}_{request.Days}_{request.Limit}";
            if (_cache.TryGetValue(cacheKey, out var cached))
                return cached;

            var apiKey = _configuration["NewsApi:ApiKey"];
            var fromDate = DateTime.UtcNow.AddDays(-request.Days!.Value).ToString("yyyy-MM-dd");

            var url = $"https://newsapi.org/v2/everything?q={request.Keyword}&from={fromDate}&language=vi&sortBy=publishedAt&apiKey={apiKey}";

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("User-Agent", "SEP490_G154_NewsService/1.0");

            var res = await _httpClient.SendAsync(req);
            var content = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception($"News API failed: {res.StatusCode} - {content}");

            var json = JObject.Parse(content);
            var articles = json["articles"]?.ToObject<List<dynamic>>() ?? new List<dynamic>();

            // ✅ Lọc thêm theo source nếu có
            if (!string.IsNullOrEmpty(request.Source))
            {
                articles = articles
                    .Where(a => a["source"]?["name"]?.ToString()?.Contains(request.Source, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }

            // ✅ Giới hạn số lượng bài viết
            var result = articles.Take(request.Limit ?? 10).ToList();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }
    }
}
