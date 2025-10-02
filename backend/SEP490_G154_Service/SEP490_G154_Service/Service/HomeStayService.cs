
using Microsoft.EntityFrameworkCore;
using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;

namespace SEP490_G154_Service.Service
{
    public class HomeStayService : IHomeStay
    {
        private readonly G154context _context;

        public HomeStayService(G154context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HomestayDTO>> FilterHeaderAsync(FilterHeaderRequestDTO request)
        {
            if (request.CheckIn >= request.CheckOut)
                throw new ArgumentException("Check-in date must be less than check-out date.");

            if (request.Rooms > request.Guests)
                throw new ArgumentException("The number of rooms cannot be greater than the number of people.");

            if (request.Guests <= 0 || request.Rooms <= 0)
                throw new ArgumentException("Number of people and number of rooms must be greater than 0.");

            var homestays = await _context.Homestays
                .Where(h => h.Status == 1)
                .Select(h => new HomestayDTO
                {
                    HomestayId = h.HomestayId,
                    Name = h.Name,
                    LocationText = h.LocationText,
                    RoomTypes = h.HomestayRoomTypes
                        .Select(rt => new RoomTypeDTO
                        {
                            RoomTypeId = rt.RoomTypeId,
                            Name = rt.Name,
                            Capacity = rt.Capacity ?? 0,
                            TotalRooms = rt.TotalRooms,
                            BasePrice = rt.BasePrice,
                            AvailableRooms = rt.TotalRooms - rt.BookingItems.Count(bi =>
                                bi.Booking.CheckinDate < request.CheckOut &&
                                bi.Booking.CheckoutDate > request.CheckIn
                            )
                        })
                        .Where(rt =>
                            rt.AvailableRooms >= request.Rooms && (
                                (request.Rooms == 1 && rt.Capacity >= request.Guests) ||
                                (request.Rooms > 1 && rt.Capacity * rt.AvailableRooms >= request.Guests)
                            )
                        )
                        .ToList()
                })
                .ToListAsync();

            return homestays;
        }
    }
}