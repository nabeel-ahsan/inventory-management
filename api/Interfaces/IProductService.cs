using System;
using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Entities;

namespace InventoryManagement.Api;

public interface IProductService
{
    Task<ProductResponseDto> GetProductsAsync(
        ProductQueryParameters queryParams
    );
    Task<(int statusCode, Product? product)> GetProductByIdAync(Guid id);

    Task<(int statusCode, Product? product)> AddProductAsync(ProductDto productDto);

    Task<(int statusCode, Product product)> UpdateProduct(Guid id, UpdateProductDto productDto);

    Task<(int statusCode, bool Success)> DeleteAsync(Guid id);

    Task<(int statusCode, Product? product)> IncreaseStockAsync(Guid id, InventoryDto dto);
    Task<(int statusCode, Product? product)> DecreaseStockAsync(Guid id, InventoryDto dto);
}
