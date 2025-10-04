using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_G154_Service.DTOs.News;
using SEP490_G154_Service.Interface;

namespace SEP490_G154_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INews _newsService;
        public NewsController(INews newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword = "Vietnam")
        {
            var articles = await _newsService.GetNewsAsync(keyword);
            return Ok(articles);
        }

        [HttpPost("refresh")]
        [Authorize(Roles = "Admin")]
        public IActionResult ClearCache()
        {
            _newsService.ClearCache();
            return Ok(new { message = "News cache has been successfully refreshed." });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] FilterNewsRequestDTO request)
        {
            var result = await _newsService.FilterNewsAsync(request);
            return Ok(result);
        }
    }
}
