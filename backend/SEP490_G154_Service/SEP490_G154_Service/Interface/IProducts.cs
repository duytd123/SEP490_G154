using SEP490_G154_Service.DTOs.Common;
using SEP490_G154_Service.DTOs.ManageProduct;

namespace SEP490_G154_Service.Interface
{
    public interface IProducts
    {
        Task<IEnumerable<ProductDTO>> GetAllProductAsync();
        Task<ProductDTO> GetProductByIdAsync(int productId);
        Task<ProductDTO> CreateProductAsync(CreateProductDTO dto, long sellerId);
        Task<ProductDTO> UpdateProductAsync(long productId, UpdateProductDTO dto, long sellerId, string role);
        Task<bool> DeleteProductAsync(long productId, long userId, string role);
        Task<bool> RestoreProductAsync(long productId, long userId, string role);
        Task<PagedResult<ProductDTO>> SearchProductsAsync(SearchProductRequestDTO request);


    }
}
