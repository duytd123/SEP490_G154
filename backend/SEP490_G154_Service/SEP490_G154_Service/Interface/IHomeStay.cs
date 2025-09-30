using SEP490_G154_Service.DTOs.FilterHomeStay;
using SEP490_G154_Service.DTOs.MaLogin;

namespace SEP490_G154_Service.Interface
{
    public interface IHomeStay
    {
        Task<IEnumerable<HomestayDTO>> FilterHeaderAsync(FilterHeaderRequestDTO request);

        Task<string> IndexAllHomestaysAsync();

        Task<IEnumerable<HomestayDTO>> FilterHeaderWithElasticAsync(FilterWithElastic request, string? keyword);
    }
}
