namespace SEP490_G154_Service.DTOs.ManageProduct
{
    public class SearchProductRequestDTO
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Stock { get; set; }
        public int? Status { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string SortBy { get; set; } = "CreatedAt"; 
        public string SortDirection { get; set; } = "desc"; 
    }
}
