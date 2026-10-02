using System;
using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Entities;

namespace InventoryManagement.Api;

public interface IProductRepository
{
   Task<(IEnumerable<Product> data, int totalCount)> GetProducts(ProductQueryParameters queryParameters);
    Task<Product> GetById(Guid id);
    Task<Product> AddProduct(Product product);
    Task<Product> UpdateProduct(Product product);
    Task<bool> DeleteProduct(Guid id);

    Task<Product> IncreaseStock(Guid id, int stockQuantity);
    Task<Product> DecreaseStock(Guid id, int stockQuantity);
}
