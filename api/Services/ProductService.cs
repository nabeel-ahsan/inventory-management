using System;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Entities;
using InventoryManagement.Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryManagement.Api;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICategoryRepository _categoryRepository; 

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _repository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductResponseDto> GetProductsAsync(ProductQueryParameters queryParams)
    {
        var (items, totalCount) = await _repository.GetProducts(queryParams);

        var mappedDtos = items
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
            })
            .ToList();

        var responseDto = new ProductResponseDto
        {
            Items = mappedDtos,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
        };

        return responseDto;
    }

    public async Task<(int statusCode, Product? product)> GetProductByIdAync(Guid id)
    {
        try
        {
            var result = await _repository.GetById(id);
            if (result == null)
            {
                return (404, null);
            }
            return (200, result);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<(int statusCode, Product? product)> AddProductAsync(ProductDto productDto)
    {
        var category = await _categoryRepository.GetById(productDto.CategoryId);
        if (category == null)
        {
            return (400, null);
        }
        var newProduct = new Product(
            productDto.Name,
            productDto.Description,
            productDto.Price ?? 0m,
            productDto.StockQuantity ?? 0,
            productDto.CategoryId
        );
        var result = await _repository.AddProduct(newProduct);
        if (result == null)
        {
            return (500, null);
        }
        return (201, result);
    }

    public async Task<(int statusCode, Product? product)> UpdateProduct(Guid id, UpdateProductDto productDto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return (400, null);
            }

            var existingProduct = await _repository.GetById(id);
            if (existingProduct == null)
            {
                return (404, null);
            }

            existingProduct.Name = productDto.Name ?? existingProduct.Name;
            existingProduct.Description = productDto.Description ?? existingProduct.Description;
            existingProduct.Price = productDto.Price ?? existingProduct.Price;
            existingProduct.CategoryId =
                productDto.CategoryId != Guid.Empty
                    ? productDto.CategoryId
                    : existingProduct.CategoryId;

            var result = await _repository.UpdateProduct(existingProduct);
            if (result == null)
            {
                return (500, null);
            }
            return (204, result);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<(int statusCode, bool Success)> DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteProduct(id);

        if (result == false)
            return (404, false);

        return (200, true);
    }

    public async Task<(int statusCode, Product? product)> IncreaseStockAsync(
        Guid id,
        InventoryDto dto
    )
    {
        if (dto.StockQuantity <= 0)
            return (400, null);

        var existingProduct = await _repository.GetById(id);

        if (existingProduct == null)
            return (404, null);

        var result = await _repository.IncreaseStock(id, dto.StockQuantity);

        if (result == null)
            return (500, null);

        return (204, null);
    }

    public async Task<(int statusCode, Product? product)> DecreaseStockAsync(
        Guid id,
        InventoryDto dto
    )
    {
        if (dto.StockQuantity <= 0)
            return (400, null);

        var existingProduct = await _repository.GetById(id);

        if (existingProduct == null)
            return (404, null);

        if (existingProduct.StockQuantity < dto.StockQuantity)
            return (400, null);

        var result = await _repository.DecreaseStock(id, dto.StockQuantity);

        if (result == null)
            return (500, null);

        return (204, existingProduct);
    }
}
