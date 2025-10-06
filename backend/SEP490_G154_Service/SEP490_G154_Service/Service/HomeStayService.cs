
using Microsoft.EntityFrameworkCore;
using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using Nest;
using SEP490_G154_Service.DTOs.ManageHomeStays;
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

        // Reindex dữ liệu từ SQL sang Elasticsearch
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
            // Validate trước khi query
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

        // ======================= GET HOMESTAY =======================

        //  Lấy toàn bộ Homestay cùng hình ảnh và loại phòng
        public async Task<IEnumerable<HomestayV1DTO>> GetAllHomestaysAsync()
        {
            var homestays = await _context.Homestays
                .Include(h => h.HomesStayImages)
                .Include(h => h.HomestayRoomTypes)
                .Where(h => h.Status == 1)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return homestays.Select(h => new HomestayV1DTO
            {
                HomestayId = h.HomestayId,
                Name = h.Name,
                Description = h.Description,
                LocationText = h.LocationText,
                Status = h.Status,
                MinPrice = h.HomestayRoomTypes.Any() ? h.HomestayRoomTypes.Min(rt => rt.BasePrice) : null,
                ThumbnailUrl = h.HomesStayImages.FirstOrDefault()?.Url ?? string.Empty,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            }).ToList();
        }
        // Lấy chi tiết Homestay theo ID, bao gồm hình ảnh và loại phòng
        public async Task<HomestayDetailDTO> GetHomestayByIdAsync(long homestayId)
        {
            var homestay = await _context.Homestays
                .Include(h => h.HomesStayImages)
                .Include(h => h.HomestayRoomTypes)
                .FirstOrDefaultAsync(h => h.HomestayId == homestayId);

            if (homestay == null)
                throw new KeyNotFoundException("Homestay not found");

            return new HomestayDetailDTO
            {
                HomestayId = homestay.HomestayId,
                Name = homestay.Name,
                Description = homestay.Description,
                LocationText = homestay.LocationText,
                Status = homestay.Status,
                CreatedAt = homestay.CreatedAt,
                UpdatedAt = homestay.UpdatedAt,
                Images = homestay.HomesStayImages.Select(i => i.Url).ToList(),
                RoomTypes = homestay.HomestayRoomTypes.Select(rt => new RoomTypeDetailDTO
                {
                    RoomTypeId = rt.RoomTypeId,
                    Name = rt.Name,
                    BasePrice = rt.BasePrice,
                    TotalRooms = rt.TotalRooms,
                    Capacity = rt.Capacity,
                    Status = rt.Status
                }).ToList()
            };
        }
        // Lấy chi tiết Homestay theo ID, bao gồm hình ảnh và loại phòng theo Host

        public async Task<IEnumerable<HomestayDetailDTO>> GetAllHomestayByHostIdAsync(long userId, string role)
        {
            IQueryable<Homestay> query = _context.Homestays
                .Include(h => h.HomesStayImages)
                .Include(h => h.HomestayRoomTypes);

            // Nếu là Host -> chỉ lấy homestay của host đó
            if (role == "Host")
            {
                query = query.Where(h => h.HostId == userId);
            }

            var homestays = await query.ToListAsync();

            if (!homestays.Any())
                throw new KeyNotFoundException("Không tìm thấy homestay nào cho host này.");

            // Map sang DTO
            return homestays.Select(h => new HomestayDetailDTO
            {
                HomestayId = h.HomestayId,
                Name = h.Name,
                Description = h.Description,
                LocationText = h.LocationText,
                Status = h.Status,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
                Images = h.HomesStayImages.Select(i => i.Url).ToList(),
                RoomTypes = h.HomestayRoomTypes.Select(rt => new RoomTypeDetailDTO
                {
                    RoomTypeId = rt.RoomTypeId,
                    Name = rt.Name,
                    BasePrice = rt.BasePrice,
                    TotalRooms = rt.TotalRooms,
                    Capacity = rt.Capacity,
                    Status = rt.Status
                }).ToList()
            }).ToList();
        }



        // Lấy danh sách Homestay đang chờ duyệt (Admin)

        public async Task<IEnumerable<HomestayDetailDTO>> GetPendingHomestaysAsync()
        {
            var homestays = await _context.Homestays
                .Include(h => h.HomesStayImages)
                .Include(h => h.HomestayRoomTypes)
                .Where(h => h.Status == 0)
                .ToListAsync();

            return homestays.Select(h => new HomestayDetailDTO
            {
                HomestayId = h.HomestayId,
                Name = h.Name,
                Description = h.Description,
                LocationText = h.LocationText,
                Status = h.Status,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
                Images = h.HomesStayImages.Select(i => i.Url).ToList(),
                RoomTypes = h.HomestayRoomTypes.Select(rt => new RoomTypeDetailDTO
                {
                    RoomTypeId = rt.RoomTypeId,
                    Name = rt.Name,
                    BasePrice = rt.BasePrice,
                    TotalRooms = rt.TotalRooms,
                    Capacity = rt.Capacity,
                    Status = rt.Status
                }).ToList()
            }).ToList();
        }

        // Lấy danh sách loại phòng đang chờ duyệt (Admin)
        public async Task<IEnumerable<RoomTypeDetailDTO>> GetPendingRoomTypesAsync()
        {
            var roomTypes = await _context.HomestayRoomTypes
                .Include(rt => rt.Homestay)
                .Where(rt => rt.Status == 0)
                .ToListAsync();

            return roomTypes.Select(rt => new RoomTypeDetailDTO
            {
                RoomTypeId = rt.RoomTypeId,
                Name = rt.Name,
                BasePrice = rt.BasePrice,
                TotalRooms = rt.TotalRooms,
                Capacity = rt.Capacity,
                Status = rt.Status,

            }).ToList();
        }



        // ======================= CREATE HOMESTAY =======================
        public async Task<HomestayDetailDTO> CreateHomestayAsync(CreateHomestayDTO dto, long hostId)
        {
            // 🔹 Check trùng tên trong phạm vi host
            bool isDuplicate = await _context.Homestays
                .AnyAsync(h => h.HostId == hostId && h.Name.ToLower() == dto.Name.ToLower());

            if (isDuplicate)
                throw new InvalidOperationException("Tên Homestay đã tồn tại. Vui lòng chọn tên khác.");

            // 🔹 Tạo Homestay mới
            var homestay = new Homestay
            {
                HostId = hostId,
                Name = dto.Name.Trim(),
                Description = dto.Description,
                LocationText = dto.LocationText,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            // 🔹 Thêm ảnh
            if (dto.ImageUrls != null && dto.ImageUrls.Any())
            {
                homestay.HomesStayImages = dto.ImageUrls.Select(url => new HomesStayImage
                {
                    Url = url
                }).ToList();
            }

            _context.Homestays.Add(homestay);
            await _context.SaveChangesAsync();

            return await GetHomestayByIdAsync(homestay.HomestayId);
        }

        // ======================= UPDATE HOMESTAY =======================
        public async Task<HomestayDetailDTO> UpdateHomestayAsync(long homestayId, UpdateHomestayDTO dto, long userId, string role)
        {
            var homestay = await _context.Homestays
                .Include(h => h.HomesStayImages)
                .FirstOrDefaultAsync(h => h.HomestayId == homestayId);

            if (homestay == null)
                throw new KeyNotFoundException("Homestay không tồn tại.");

            // 🔹 Kiểm tra quyền
            if (role == "Host" && homestay.HostId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền sửa Homestay này.");

            // 🔹 Kiểm tra trùng tên (ngoại trừ chính nó)
            bool duplicateName = await _context.Homestays
                .AnyAsync(h => h.HostId == homestay.HostId && h.Name.ToLower() == dto.Name.ToLower() && h.HomestayId != homestayId);

            if (duplicateName)
                throw new InvalidOperationException("Tên Homestay đã tồn tại. Vui lòng chọn tên khác.");

            // 🔹 Cập nhật thông tin
            homestay.Name = dto.Name.Trim();
            homestay.Description = dto.Description;
            homestay.LocationText = dto.LocationText;
            homestay.UpdatedAt = DateTime.UtcNow;
            homestay.Status = dto.Status;

            // 🔹 Cập nhật ảnh (xóa cũ, thêm mới)
            if (dto.ImageUrls != null && dto.ImageUrls.Any())
            {
                _context.HomesStayImages.RemoveRange(homestay.HomesStayImages);
                homestay.HomesStayImages = dto.ImageUrls.Select(url => new HomesStayImage
                {
                    Url = url
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return await GetHomestayByIdAsync(homestay.HomestayId);
        }


        // ======================= CREATE ROOM TYPE =======================


        public async Task<object> CreateRoomTypeAsync(CreateRoomTypeDTO dto, long homestayId, long userId, string role)
        {
            Homestay? homestay;

            if (role == "Admin")
                homestay = await _context.Homestays.FirstOrDefaultAsync(h => h.HomestayId == homestayId);
            else
                homestay = await _context.Homestays.FirstOrDefaultAsync(h => h.HomestayId == homestayId && h.HostId == userId);

            if (homestay == null)
                throw new KeyNotFoundException("Homestay không tồn tại hoặc bạn không có quyền thêm phòng.");

            // check trùng tên
            bool exists = await _context.HomestayRoomTypes
                .AnyAsync(rt => rt.HomestayId == homestayId && rt.Name == dto.Name);

            if (exists)
                throw new InvalidOperationException("Tên RoomType đã tồn tại trong homestay này.");

            var roomType = new HomestayRoomType
            {
                HomestayId = homestayId,
                Name = dto.Name,
                BasePrice = dto.BasePrice,
                TotalRooms = dto.TotalRooms,
                Capacity = dto.Capacity,
                Status = 0
            };

            _context.HomestayRoomTypes.Add(roomType);
            await _context.SaveChangesAsync();


            var result = new
            {
                message = "RoomType created successfully",
                roomType = new
                {
                    roomType.RoomTypeId,
                    roomType.HomestayId,
                    roomType.Name,
                    roomType.BasePrice,
                    roomType.TotalRooms,
                    roomType.Capacity,
                    roomType.Status
                }
            };

            return result;
        }


        // ======================= UPDATE ROOM TYPE =======================
        public async Task<RoomTypeDetailDTO> UpdateRoomTypeAsync(long roomTypeId, UpdateRoomTypeDTO dto, long userId, string role)
        {
            var roomType = await _context.HomestayRoomTypes
                .Include(rt => rt.Homestay)
                .FirstOrDefaultAsync(rt => rt.RoomTypeId == roomTypeId);

            if (roomType == null)
                throw new KeyNotFoundException("Loại phòng không tồn tại.");

            if (role == "Host" && roomType.Homestay.HostId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền sửa loại phòng này.");

            // 🔹 Kiểm tra trùng tên (trong cùng homestay, trừ chính nó)
            bool duplicateName = await _context.HomestayRoomTypes
                .AnyAsync(rt => rt.HomestayId == roomType.HomestayId && rt.Name.ToLower() == dto.Name.ToLower() && rt.RoomTypeId != roomTypeId);

            if (duplicateName)
                throw new InvalidOperationException("Tên loại phòng đã tồn tại trong homestay này.");

            // 🔹 Cập nhật dữ liệu
            roomType.Name = dto.Name.Trim();
            roomType.BasePrice = dto.BasePrice;
            roomType.TotalRooms = dto.TotalRooms;
            roomType.Capacity = dto.Capacity;
            roomType.Status = dto.Status;

            await _context.SaveChangesAsync();

            return new RoomTypeDetailDTO
            {
                RoomTypeId = roomType.RoomTypeId,
                Name = roomType.Name,
                BasePrice = roomType.BasePrice,
                TotalRooms = roomType.TotalRooms,
                Capacity = roomType.Capacity,
                Status = roomType.Status
            };
        }


        public async Task<bool> DeleteHomestayAsync(long homestayId, long userId, string role)
        {
            var query = _context.Homestays.AsQueryable();
            if (role == "Host")
                query = query.Where(h => h.HostId == userId);

            var homestay = await query.FirstOrDefaultAsync(h => h.HomestayId == homestayId);
            if (homestay == null)
                throw new KeyNotFoundException("Homestay not found or not authorized");

            homestay.Status = 0;
            homestay.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ApproveHomestayAsync(long homestayId, int newStatus)
        {
            var homestay = await _context.Homestays.FindAsync(homestayId);
            if (homestay == null) throw new KeyNotFoundException("Homestay not found");

            homestay.Status = newStatus;
            homestay.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ApproveRoomTypeAsync(long roomTypeId, int newStatus)
        {
            // Tìm RoomType theo ID
            var roomType = await _context.HomestayRoomTypes.FindAsync(roomTypeId);
            if (roomType == null)
                throw new KeyNotFoundException("Room type not found");

            // Cập nhật trạng thái
            roomType.Status = newStatus;

            // Optional: cập nhật UpdatedAt của Homestay nếu muốn log chung
            var homestay = await _context.Homestays.FindAsync(roomType.HomestayId);
            if (homestay != null)
            {
                homestay.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}