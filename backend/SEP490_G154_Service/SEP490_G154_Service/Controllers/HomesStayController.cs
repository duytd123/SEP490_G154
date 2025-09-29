using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_G154_Service.DTOs;
using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;

namespace SEP490_G154_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomesStayController : ControllerBase
    {
        private readonly IHomeStay _homeStayService;

        public HomesStayController(IHomeStay homeStayService)
        {
            _homeStayService = homeStayService;
        }

        [HttpPost("FilterHeader")]
        public async Task<IActionResult> FilterHeader([FromBody] FilterHeaderRequestDTO request)
        {
            try
            {
                var result = await _homeStayService.FilterHeaderAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}