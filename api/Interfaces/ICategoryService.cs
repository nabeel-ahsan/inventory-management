using System;
using InventoryManagement.Api.Entities;

namespace InventoryManagement.Api.Interfaces;

public interface ICategoryService
{
Task<List<CategoryResponseDto>> GetCategoriesAsync();
Task<(int statusCode, CategoryResponseDto? responseDto)> GetCategoryByIdAync(Guid id);
Task<(int statusCode, CategoryResponseDto? responseDto)> AddCategoryAsync(CreateCategoryDto categoryDto);
Task<(int statusCode, CategoryResponseDto? responseDto)> UpdateCategory(CreateCategoryDto categoryDto);
Task<(int statusCode, string message)> DeleteAsync(Guid id);
}
