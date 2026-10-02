using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _service;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _service = productService;
        }

        [HttpGet("getAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] ProductQueryParameters queryParams
        )
        {
            _logger.LogInformation(
                "Fetching all products with query parameters: {@QueryParams}",
                queryParams
            );

            try{var result = await _service.GetProductsAsync(queryParams);
            return Ok(result);}
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to retrieve products due to an unexpected error.");
            throw;
            }
        }

        [HttpGet("getProductById/{id:Guid}")]
        [Authorize] // 🔒 Endpoints with this attribute now demand a valid JWT in the Header
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching product by id: {id}", id);
                var (statusCode, product) = await _service.GetProductByIdAync(id);
                if (statusCode == 404)
                {
                    return NotFound($"The product with id = {id} was not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to retrieve product with id = {id} due to an unexpected error.", id);
            throw;
            }
        }

        [HttpPost("addProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
        {
            _logger.LogInformation("Adding a new product: {@ProductDto}", productDto);
            try
            {
                var (statusCode, addedProduct) = await _service.AddProductAsync(productDto);
                if (statusCode == 500)
                {
                    return Problem("An error occurred while creating the product.");
                }
                if (statusCode == 400)
                {
                    return BadRequest("Invalid category ID. The specified category does not exist.");
                }
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = addedProduct.Id },
                    addedProduct
                );
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to add product due to an unexpected error.");
            throw;
            }
        }

        [HttpPut("updateProduct/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto productDto)
        {
            _logger.LogInformation("Updating product: {@ProductDto}", productDto);
            try{var (statusCode, product) = await _service.UpdateProduct(id, productDto);
            if (statusCode == 404)
            {
                return NotFound($"The product with id = {id} was not found.");
            }
            if (statusCode == 400)
            {
                return BadRequest("A valid product id is required.");
            }
            if (statusCode == 204)
            {
                return Ok(product);
            }
            return Problem("An error occurred while updating the product.");}
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to update product with id = {id} due to an unexpected error.", id);
            throw;
            }
        }

        [HttpDelete("deleteProduct/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _logger.LogInformation("Deleting product with id: {id}", id);
            try
            {
                var (statusCode, success) = await _service.DeleteAsync(id);
                if (statusCode == 404)
                    return NotFound();
                return Ok(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete product with id: {id} due to an unexpected error.",
                    id
                );
                throw;
            }
        }

        [HttpPatch("stock/increase/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IncreaseStock(
            [FromRoute] Guid id,
            [FromBody] InventoryDto dto
        )
        {
            _logger.LogInformation(
                "Increasing stock for product with id: {id} by quantity: {quantity}",
                id,
                dto.StockQuantity
            );
            try
            {
                var (statusCode, product) = await _service.IncreaseStockAsync(id, dto);

                if (statusCode == 400)
                    return BadRequest("Invalid stock quantity");

                if (statusCode == 404)
                    return NotFound($"The product with id: {id} was not found");

                if (statusCode == 500)
                    return StatusCode(500, "An error occurred while processing your request.");

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to increase stock for product with id: {id} due to an unexpected error.",
                    id
                );
                throw;
            }
        }

        [HttpPatch("stock/decrease/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DecreaseStock(
            [FromRoute] Guid id,
            [FromBody] InventoryDto dto
        )
        {
            _logger.LogInformation(
                "Decreasing stock for product with id: {id} by quantity: {quantity}",
                id,
                dto.StockQuantity
            );
            try
            {
                var (statusCode, product) = await _service.DecreaseStockAsync(id, dto);

                if (statusCode == 400)
                    return BadRequest("Invalid stock quantity");

                if (statusCode == 404)
                    return NotFound($"The product with id: {id} was not found");

                if (statusCode == 500)
                    return StatusCode(500, "An error occurred while processing your request.");

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to decrease stock for product with id: {id} due to an unexpected error.",
                    id
                );
                throw;
            }
        }
    }
}
