using System;

namespace InventoryManagement.Api.DTOs;

public class ProductResponseDto
{
    // The actual array of product data
    public IEnumerable<ProductDto> Items { get; set; } = new List<ProductDto>();

    // The pagination metadata fields
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    // public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

