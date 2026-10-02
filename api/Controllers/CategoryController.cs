using InventoryManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _service;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _service = categoryService;
        }

        [HttpGet("getAllCategories")]
        [Authorize] // 🔒 Endpoints with this attribute now demand a valid JWT in the Header
        public async Task<IActionResult> GetAllCategories()
        {
            _logger.LogInformation("Fetching all categories.");
            try{var result = await _service.GetCategoriesAsync();
            return Ok(result);}
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to retrieve categories due to an unexpected error.");
            throw;  }
        }

        [HttpGet("getCategoryById/{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            _logger.LogInformation("Fetching category by id: {id}", id);
            try
            {
                var (statusCode, category) = await _service.GetCategoryByIdAync(id);
                if (statusCode == 404)
                {
                    return NotFound($"The category with id = {id} was not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to retrieve category with id = {id} due to an unexpected error.", id);
            throw;  }
        }

        [HttpPost("addCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto categoryDto)
        {
            _logger.LogInformation("Adding a new category.");
            try
            {
                var (statusCode, addedCategory) = await _service.AddCategoryAsync(categoryDto);
                if (statusCode == 500)
                {
                    return Problem("An error occurred while creating the category.");
                }
                return CreatedAtAction(nameof(GetCategoryById), addedCategory);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to add category due to an unexpected error.");
            throw;  }
        }

        [HttpPut("updateCategory")]
        [Authorize(Roles = "Admin")] // 🔒 Endpoints with this attribute now demand a valid JWT in the Header
        public async Task<IActionResult> UpdateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            _logger.LogInformation("Updating category with id: {id}", categoryDto.Id);
            try{var (statusCode, category) = await _service.UpdateCategory(categoryDto);
            if(statusCode == 404)
            {
                return NotFound($"The category with id = {categoryDto.Id} was not found.");
            }
            if (statusCode == 400)
            {
                return BadRequest("A valid category id is required.");
            }
            if(statusCode == 204)
            {
                return Ok(category);
            }
            return Problem("An error occurred while updating the category.");}
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to update category with id = {id} due to an unexpected error.", categoryDto.Id);
            throw;  }
        }

        [HttpDelete("deleteCategory/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id){
            _logger.LogInformation("Deleting category with id: {id}", id); 
            try{var (statusCode, message) = await _service.DeleteAsync(id);
            if(statusCode == 500) return Problem(message);
            if(statusCode == 409) return Problem(message);
            if(statusCode == 404) return NotFound(message);
            return Ok(message);}
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to delete category with id = {id} due to an unexpected error.", id);
            throw;  }
        }
    }
}
