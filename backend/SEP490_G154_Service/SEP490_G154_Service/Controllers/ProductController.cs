using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_G154_Service.DTOs.ManageProduct;
using SEP490_G154_Service.Interface;
using System.Security.Claims;

namespace SEP490_G154_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProducts _products;
        public ProductController(IProducts products)
        {
            _products = products;
        }

        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
            => Ok(await _products.GetAllProductAsync());


        [HttpGet("GetProductById/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
            => Ok(await _products.GetProductByIdAsync(productId));


        [HttpPost("CreateProduct")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO dto)
        {
            // lấy sellerId từ JWT token
            var sellerId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var product = await _products.CreateProductAsync(dto, sellerId);
            return Ok(product);
        }

        [HttpPut("UpdateProduct/{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductDTO dto)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
              if (role == null)
            {
                return BadRequest(new { message = "User role is missing." });
            }

            var product = await _products.UpdateProductAsync(id, dto, userId, role);
            return Ok(product);
        }

        [HttpDelete("DeleteProduct/{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == null)
            {
                return BadRequest(new { message = "User role is missing." });
            }

            var result = await _products.DeleteProductAsync(id, userId, role);

            return result ? Ok(new { message = "Product has been deactivated." })
                          : BadRequest(new { message = "Failed to deactivate product." });
        }

        [HttpPut("Restore/{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Restore(long id)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

            var result = await _products.RestoreProductAsync(id, userId, role);
            return Ok(new { success = result, message = "Product restored successfully" });
        }


        [HttpPost("SearchProducts")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchProductRequestDTO request)
        {
            var result = await _products.SearchProductsAsync(request);
            return Ok(result);
        }



    }
}
