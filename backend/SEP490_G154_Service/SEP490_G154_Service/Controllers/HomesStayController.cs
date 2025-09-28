using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_G154_Service.DTOs;
using SEP490_G154_Service.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SEP490_G154_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomesStayController : ControllerBase
    {
        private readonly G154context _context;
        public HomesStayController(G154context context)
        {
            _context = context;
        }

        [HttpGet("FilterHeader")]
        public IActionResult FilterHeader(DateOnly checkIn, DateOnly checkOut, int guests, int rooms)
        {
            if (checkIn >= checkOut)
                return BadRequest("Ngày check-in phải nhỏ hơn ngày check-out.");

            var homestays = _context.Homestays
                .Where(h => h.Status == 1)
                .Select(h => new HomestayDTO
                {
                    HomestayId = h.HomestayId,
                    Name = h.Name,
                    LocationText = h.LocationText,
                    RoomTypes = h.HomestayRoomTypes
                        .Where(rt => rt.Capacity >= guests)
                        .Select(rt => new RoomTypeDTO
                        {
                            RoomTypeId = rt.RoomTypeId,
                            Name = rt.Name,
                            Capacity = rt.Capacity ?? 0,
                            TotalRooms = rt.TotalRooms,
                            BasePrice = rt.BasePrice,

                            // Tính số phòng còn trống trong khoảng thời gian đã cho
                            AvailableRooms = rt.TotalRooms - rt.BookingItems.Count(bi =>
                                bi.Booking.CheckinDate < checkOut &&
                                bi.Booking.CheckoutDate > checkIn
                            )
                        })
                        .Where(rt => rt.AvailableRooms >= rooms)
                        .ToList()
                })
                .ToList();

            return Ok(homestays);
        }


      

    }
}
