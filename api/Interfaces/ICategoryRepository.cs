using System;
using InventoryManagement.Api.Entities;

namespace InventoryManagement.Api.Interfaces;

public interface ICategoryRepository
{
Task<IEnumerable<Category>> GetCategories();
Task<Category> GetById(Guid id);
Task<Category> AddCategory(Category category);
Task<Category> UpdateCategory(Category category);
Task<bool> DeleteCategory(Guid id);
Task<bool> HasProducts(Guid categoryId);
}
