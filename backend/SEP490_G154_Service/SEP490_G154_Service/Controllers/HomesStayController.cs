using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_G154_Service.DTOs;
using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.DTOs.ManageHomeStays;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using SEP490_G154_Service.Service;
using System.Security.Claims;

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


        [HttpPost("Reindex")]
        public async Task<IActionResult> Reindex()
        {
            var result = await _homeStayService.IndexAllHomestaysAsync();
            return Ok(result);
        }

        [HttpPost("SearchElastic")]
        public async Task<IActionResult> SearchElastic([FromBody] FilterWithElastic request, [FromQuery] string? keyword)
        {
            var result = await _homeStayService.FilterHeaderWithElasticAsync(request, keyword);
            return Ok(result);
        }
        [HttpGet("GetAllHomestays")]
        public async Task<IActionResult> GetAllHomestays()
        {
            var result = await _homeStayService.GetAllHomestaysAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _homeStayService.GetHomestayByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("getall-homestays-host")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> GetAllHomestaysByHost()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var result = await _homeStayService.GetAllHomestayByHostIdAsync(userId, role);
            return Ok(result);
        }



        [HttpPost("create")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateHomestayDTO dto)
        {
            var hostId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _homeStayService.CreateHomestayAsync(dto, hostId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateHomestayDTO dto)
        {
            var hostId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var result = await _homeStayService.UpdateHomestayAsync(id, dto, hostId, role);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var hostId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            await _homeStayService.DeleteHomestayAsync(id, hostId, role);
            return Ok(new { message = "Homestay deactivated" });
        }
        [HttpPost("{homestayId}/add-roomtype")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> AddRoomType(long homestayId, [FromBody] CreateRoomTypeDTO dto)
        {
            var hostId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            // 🔧 Sửa đúng thứ tự tham số
            var result = await _homeStayService.CreateRoomTypeAsync(dto, homestayId, hostId, role);
            return Ok(result);
        }



        [HttpPut("update-roomtype/{id}")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> UpdateRoomType(long id, [FromBody] UpdateRoomTypeDTO dto)
        {
            var hostId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var result = await _homeStayService.UpdateRoomTypeAsync(id, dto, hostId, role);
            return Ok(result);
        }

        [HttpDelete("delete-roomtype/{id}")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> DeleteRoomType(long id)
        {
            var hostId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            await _homeStayService.DeleteHomestayAsync(id, hostId, role);
            return Ok(new { message = "Room type deactivated" });
        }

        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(long id)
        {
            var result = await _homeStayService.ApproveHomestayAsync(id, 1);
            return Ok(new { message = "Homestay approved", result });
        }

        [HttpPut("approve-roomtype/{roomTypeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRoomType(long roomTypeId, [FromQuery] int status)
        {
            var result = await _homeStayService.ApproveRoomTypeAsync(roomTypeId, status);
            return Ok(new { success = result, message = "RoomType status updated successfully." });
        }

        [HttpGet("pending-homestays")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingHomestays()
        {
            var result = await _homeStayService.GetPendingHomestaysAsync();
            return Ok(result);
        }

        [HttpGet("pending-roomtypes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRoomTypes()
        {
            var result = await _homeStayService.GetPendingRoomTypesAsync();
            return Ok(result);
        }

    }
}