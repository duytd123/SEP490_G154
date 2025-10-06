using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.DTOs.ManageHomeStays;

namespace SEP490_G154_Service.Interface
{
    public interface IHomeStay
    {
        Task<IEnumerable<HomestayDTO>> FilterHeaderAsync(FilterHeaderRequestDTO request);
        Task<string> IndexAllHomestaysAsync();
        Task<IEnumerable<HomestayDTO>> FilterHeaderWithElasticAsync(FilterWithElastic request, string? keyword);
        Task<IEnumerable<HomestayV1DTO>> GetAllHomestaysAsync();
        Task<HomestayDetailDTO> GetHomestayByIdAsync(long homestayId);
        Task<bool> ApproveHomestayAsync(long homestayId, int newStatus);
        Task<HomestayDetailDTO> CreateHomestayAsync(CreateHomestayDTO dto, long hostId);
        Task<object> CreateRoomTypeAsync(CreateRoomTypeDTO dto, long homestayId, long userId, string role);
        Task<HomestayDetailDTO> UpdateHomestayAsync(long homestayId, UpdateHomestayDTO dto, long userId, string role);
        Task<RoomTypeDetailDTO> UpdateRoomTypeAsync(long roomTypeId, UpdateRoomTypeDTO dto, long userId, string role);
        Task<bool> DeleteHomestayAsync(long homestayId, long userId, string role);
        Task<bool> ApproveRoomTypeAsync(long roomTypeId, int newStatus);
        Task<IEnumerable<HomestayDetailDTO>> GetPendingHomestaysAsync();
        Task<IEnumerable<RoomTypeDetailDTO>> GetPendingRoomTypesAsync();
        Task<IEnumerable<HomestayDetailDTO>> GetAllHomestayByHostIdAsync(long userId, string role);

    }
}
