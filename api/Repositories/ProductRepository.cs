using System;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Product> data, int totalCount)> GetProducts(ProductQueryParameters queryParameters)
    {
        IQueryable<Product> query = _context.Products;
        query = query.Where(p => p.IsActive); 

        if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
        {
            var searchTerm = queryParameters.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm)
                || p.Description.ToLower().Contains(searchTerm)
            );
        }
        if (queryParameters.FilterId != null)
        {
            query = query.Where(p => p.CategoryId == queryParameters.FilterId);
        }
        if (queryParameters.SortBy != null || queryParameters.SortOrder != null)
        {
            var sortBy = queryParameters.SortBy?.ToLower().Trim();
            bool isDescending = queryParameters.SortOrder?.ToLower().Trim() == "desc";

            query = sortBy switch
            {
                "price" => isDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "name" => isDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "date" => isDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                "stockQuantity" => isDescending
                    ? query.OrderByDescending(p => p.StockQuantity)
                    : query.OrderBy(p => p.StockQuantity),
                _ => query.OrderBy(p => p.Id),
            };
        }

        int totalCount = await query.CountAsync();

        int pageNumber = queryParameters.PageNumber < 1 ? 1 : queryParameters.PageNumber;
        int pageSize = queryParameters.PageSize < 1 ? 10 : queryParameters.PageSize;

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product> GetById(Guid id)
    {
        
        var result = await _context.Products.Where(p=>p.IsActive).FirstOrDefaultAsync(p => p.Id == id);
        return result;
    }

    public async Task<Product> AddProduct(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        var existingProduct = await _context.Products.Where(p=>p.IsActive).FirstOrDefaultAsync(p => p.Id == product.Id);

        if (existingProduct != null)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
        return null;
    }

    public async Task<bool> DeleteProduct(Guid id)
    {
        var existingProduct = await _context.Products.Where(p=>p.IsActive).FirstOrDefaultAsync(p => p.Id == id);
        if (existingProduct == null)
            return false;

        existingProduct.IsActive = false;
        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Product> IncreaseStock(Guid id, int stockQuantity)
    {
        var existingProduct = await _context.Products.Where(p=>p.IsActive).FirstOrDefaultAsync(p => p.Id == id);

        if (existingProduct == null)
            return null;

        existingProduct.StockQuantity += stockQuantity;
        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<Product> DecreaseStock(Guid id, int stockQuantity)
    {
        var existingProduct = await _context.Products.Where(p=>p.IsActive).FirstOrDefaultAsync(p => p.Id == id);

        if (existingProduct == null)
            return null;

        existingProduct.StockQuantity -= stockQuantity;
        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();
        return existingProduct;
    }
}
