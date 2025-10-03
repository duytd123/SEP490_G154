using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using Nest;
using SEP490_G154_Service.DTOs.Common;
using SEP490_G154_Service.DTOs.ManageProduct;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;

namespace SEP490_G154_Service.Service
{
    public class ProductService : IProducts
    {
        private readonly G154context _context;
        private readonly IElasticClient _elasticClient;
        public ProductService(G154context context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductAsync()
        {
            var products = await _context.Products
                 .Where(p => !p.IsDeleted)
                .Include(p => p.Categories)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return products.Select(p => new ProductDTO
            {
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = (double)p.Price,
                Stock = p.Stock,
                Category = p.Categories.FirstOrDefault()?.Name ?? "Uncategorized",
                ImageUrl = p.ProductImages.FirstOrDefault()?.Url ?? string.Empty,
                IsActive = p.Status == 1 && !p.IsDeleted
            }).ToList();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                 .Where(p => !p.IsDeleted)
                .Include(p => p.Categories)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            return new ProductDTO
            {
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = (double)product.Price,
                Stock = product.Stock,
                Category = product.Categories.FirstOrDefault()?.Name ?? "Uncategorized",
                ImageUrl = product.ProductImages.FirstOrDefault()?.Url ?? string.Empty,
                IsActive = product.Status == 1 && !product.IsDeleted
            };


        }


        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO dto, long sellerId)
        {
            var product = new Product
            {
                SellerId = sellerId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                Status = 1,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // gắn category
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category != null)
            {
                product.Categories.Add(category);
            }

            // gắn image
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                product.ProductImages.Add(new ProductImage
                {
                    Url = dto.ImageUrl,
                    SortOrder = 1
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductDTO
            {
                Name = product.Name,
                Description = product.Description,
                Price = (double)product.Price,
                Stock = product.Stock,
                Category = category?.Name ?? "Uncategorized",
                ImageUrl = dto.ImageUrl,
                IsActive = true
            };
        }

        public async Task<ProductDTO> UpdateProductAsync(long productId, UpdateProductDTO dto, long userId, string role)
        {
            // Nếu là Admin thì không cần check SellerId
            var query = _context.Products.AsQueryable();

            if (role == "Seller")
            {
                // Seller chỉ được update sản phẩm của mình
                query = query.Where(p => p.SellerId == userId);
            }

            var product = await query.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found or you are not authorized to update this product.");

            // Cập nhật thông tin
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = (decimal)dto.Price;
            product.Stock = dto.Stock;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ProductDTO
            {
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = (double)product.Price,
                Stock = product.Stock,
                Category = product.Categories.FirstOrDefault()?.Name ?? "Uncategorized",
                ImageUrl = product.ProductImages.FirstOrDefault()?.Url ?? string.Empty,
                IsActive = product.Status == 1 && !product.IsDeleted
            };
        }

        public async Task<bool> DeleteProductAsync(long productId, long userId, string role)
        {
            // Nếu là Admin thì không cần check SellerId
            var query = _context.Products.AsQueryable();

            if (role == "Seller")
            {
                // Seller chỉ được xóa (ẩn) sản phẩm của chính mình
                query = query.Where(p => p.SellerId == userId);
            }

            var product = await query.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found or you are not authorized to delete this product.");

            // Chỉ set status về 0 thay vì xóa
            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestoreProductAsync(long productId, long userId, string role)
        {
            var query = _context.Products.AsQueryable();

            if (role == "Seller")
            {
                // Seller chỉ được khôi phục sản phẩm của mình
                query = query.Where(p => p.SellerId == userId);
            }

            var product = await query.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found or you are not authorized to restore this product.");

            // Đánh dấu là chưa xóa
            product.IsDeleted = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<ProductDTO>> SearchProductsAsync(SearchProductRequestDTO request)
        {
            var query = _context.Products
                .Include(p => p.Categories)
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Lọc theo tên
            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(p => p.Name.Contains(request.Name));

            // Lọc theo giá
            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            // Lọc theo stock
            if (request.Stock.HasValue)
                query = query.Where(p => p.Stock >= request.Stock.Value);

            // Lọc theo status
            if (request.Status.HasValue)
                query = query.Where(p => p.Status == request.Status.Value);

            // Chỉ lấy sản phẩm chưa bị xóa
            query = query.Where(p => !p.IsDeleted);

            // ================= Sort =================
            query = (request.SortBy.ToLower(), request.SortDirection.ToLower()) switch
            {
                ("price", "asc") => query.OrderBy(p => p.Price),
                ("price", "desc") => query.OrderByDescending(p => p.Price),

                ("stock", "asc") => query.OrderBy(p => p.Stock),
                ("stock", "desc") => query.OrderByDescending(p => p.Stock),

                ("status", "asc") => query.OrderBy(p => p.Status),
                ("status", "desc") => query.OrderByDescending(p => p.Status),

                ("name", "asc") => query.OrderBy(p => p.Name),
                ("name", "desc") => query.OrderByDescending(p => p.Name),

                // Mặc định sort theo CreatedAt desc
                ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Tổng số record trước khi phân trang
            var totalCount = await query.CountAsync();

            // Phân trang
            var products = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = products.Select(p => new ProductDTO
            {
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = (double)p.Price,
                Stock = p.Stock,
                Category = p.Categories.FirstOrDefault()?.Name ?? "Uncategorized",
                ImageUrl = p.ProductImages.FirstOrDefault()?.Url ?? string.Empty,
                IsActive = p.Status == 1 && !p.IsDeleted
            }).ToList();

            return new PagedResult<ProductDTO>(items, totalCount, request.PageNumber, request.PageSize);
        }

    }
}
