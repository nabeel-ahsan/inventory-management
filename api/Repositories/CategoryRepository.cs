using System;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.Entities;
using InventoryManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category> GetById(Guid id)
    {
        var result = await _context.Categories.FirstOrDefaultAsync(p=>p.Id == id);
        return result;
    }

    public async Task<Category> AddCategory(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategory(Category category){
        var existingProduct = await _context.Categories.FirstOrDefaultAsync(p=>p.Id == category.Id);

        if(existingProduct != null)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
        return null;
    }

    public async Task<bool> DeleteCategory(Guid id)
    {
        var existingCategory = await _context.Categories.FindAsync(id);
        if(existingCategory == null) return false;

        _context.Categories.Remove(existingCategory);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasProducts(Guid categoryId)
    {
        return await _context.Products.AnyAsync(p=>p.CategoryId == categoryId);
    }
}
