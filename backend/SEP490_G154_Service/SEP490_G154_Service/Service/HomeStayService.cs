
using Microsoft.EntityFrameworkCore;
using Nest;
using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;

namespace SEP490_G154_Service.Service
{
    public class HomeStayService : IHomeStay
    {
        private readonly G154context _context;
        private readonly IElasticClient _elasticClient;
        public HomeStayService(G154context context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
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
        // ✅ Reindex dữ liệu từ SQL sang Elasticsearch
        public async Task<string> IndexAllHomestaysAsync()
        {
            var homestays = await _context.Homestays
                .Include(h => h.HomestayRoomTypes)
                .ToListAsync();

            var docs = homestays.Select(h => new HomestayDTO
            {
                HomestayId = h.HomestayId,
                Name = h.Name,
                LocationText = h.LocationText,
                RoomTypes = h.HomestayRoomTypes.Select(rt => new RoomTypeDTO
                {
                    RoomTypeId = rt.RoomTypeId,
                    Name = rt.Name,
                    Capacity = rt.Capacity ?? 0,
                    TotalRooms = rt.TotalRooms,
                    BasePrice = rt.BasePrice
                }).ToList()
            }).ToList();

            var response = await _elasticClient.IndexManyAsync(docs, "homestays");

            if (response.Errors)
            {
                var errorItems = response.ItemsWithErrors.Select(e =>
                    $"Index: {e.Index}, Id: {e.Id}, Error: {e.Error?.Reason}"
                );
                throw new Exception("Indexing errors: " + string.Join("; ", errorItems));
            }

            return $"Indexed {docs.Count} homestays into homestays";
        }

        public async Task<IEnumerable<HomestayDTO>> FilterHeaderWithElasticAsync(FilterWithElastic request, string? keyword)
        {
            // ✅ Validate trước khi query
            if (request.CheckIn >= request.CheckOut)
                throw new ArgumentException("Ngày check-in phải nhỏ hơn ngày check-out.");

            if (request.Rooms > request.Guests)
                throw new ArgumentException("Số phòng không được lớn hơn số khách.");

            if (request.Guests <= 0 || request.Rooms <= 0)
                throw new ArgumentException("Số người và số phòng phải lớn hơn 0.");

            var response = await _elasticClient.SearchAsync<HomestayDTO>(s => s
                .Index("homestays")
                .Query(q =>
                    q.Bool(b => b.Must(
                        // Keyword search (name, location, roomTypes)
                        m => string.IsNullOrEmpty(keyword)
                            ? m.MatchAll()
                            : m.MultiMatch(mm => mm
                                .Fields(f => f
                                    .Field(h => h.Name)
                                    .Field(h => h.LocationText)
                                    .Field("roomTypes.name") // field con
                                )
                                .Query(keyword)
                                .Fuzziness(Fuzziness.Auto)
                            ),

                        //  Capacity: đảm bảo mỗi phòng có thể chứa trung bình số khách
                        m => m.Range(r => r
                            .Field("roomTypes.capacity")
                            .GreaterThanOrEquals(request.Guests / request.Rooms)
                        ),

                        //  Giá filter (mặc định 0–2tr nếu chưa truyền)
                        m => m.Range(r => r
                            .Field("roomTypes.basePrice")
                            .GreaterThanOrEquals(request.MinPrice ?? 0)
                            .LessThanOrEquals(request.MaxPrice ?? 2000000)
                        )
                    ))
                )
            );

            if (!response.IsValid)
                throw new Exception("Search failed: " + response.DebugInformation);

            return response.Documents.ToList();
        }
    }
}