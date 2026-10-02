using System;
using InventoryManagement.Api.Entities;
using InventoryManagement.Api.Interfaces;

namespace InventoryManagement.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _repository = categoryRepository;
    }

    public async Task<List<CategoryResponseDto>> GetCategoriesAsync()
    {
        var items = await _repository.GetCategories();
        var response = items.Select(p=>new CategoryResponseDto{Name = p.Name}).ToList();
        return response;
    }

    public async Task<(int statusCode, CategoryResponseDto? responseDto)> GetCategoryByIdAync(Guid id)
    {
        try
        {
            var result = await _repository.GetById(id);
            if(result == null)
            {
                return (404, null);
            }
            var responseDto = new CategoryResponseDto{Name = result.Name};

            return (200, responseDto);
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

    public async Task<(int statusCode, CategoryResponseDto? responseDto)> AddCategoryAsync(CreateCategoryDto categoryDto)
    {
        try
        {
            var newCategory = new Category(categoryDto.Name);
            var result = await _repository.AddCategory(newCategory);
            if(result == null)
            {
                return (500, null);
            }
            var responseDto = new CategoryResponseDto{Name = result.Name};
            return (201, responseDto);
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

    public async Task<(int statusCode, CategoryResponseDto? responseDto)> UpdateCategory(CreateCategoryDto categoryDto)
    {
        try
        {
            if (categoryDto.Id == Guid.Empty)
            {
                return (400, null);
            }

            var existingCategory = await _repository.GetById(categoryDto.Id);
            if(existingCategory == null)
            {
                return (404, null);
            }

            existingCategory.Name = categoryDto.Name ?? existingCategory.Name;

            var result = await _repository.UpdateCategory(existingCategory);

            if(result == null)
            {
                return (500, null);
            }          
            var responseDto = new CategoryResponseDto{Name = result.Name};
            return (204, responseDto);
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

    public async Task<(int statusCode, string message)> DeleteAsync(Guid id){
        var isUsed = await _repository.HasProducts(id);
        if(isUsed) return (409, "Cannot delete category because it is associated with existing products.");
        var result = await _repository.DeleteCategory(id);

        if(result == false) return (404, "The category does not exist");
        else if(result == false) return (500, "An error occurred while deleting the category.");
        else return (201, "The category was deleted successfully!");
    }
}
