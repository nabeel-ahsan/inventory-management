using System;
using System.Reflection;
using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Entities;
using InventoryManagement.Api.Interfaces;
using Moq;

namespace InventoryManagement.Api.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        var id = Guid.NewGuid();
        var product = new Product("Laptop", "Test Laptop", 50000m, 10, Guid.NewGuid());
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(product);

        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        var result = await service.GetProductByIdAync(id);

        Assert.Equal(200, result.statusCode);
        Assert.Equal(product, result.product);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((Product?)null);
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.GetProductByIdAync(id);

        // Assert
        Assert.Equal(404, result.statusCode);
        Assert.Null(result.product);
    }

    [Fact]
    public async Task DecreaseStockAsync_WhenDecreaseExceedsStock_ReturnsBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product("Laptop", "Test laptop", 50000m, 10, Guid.NewGuid());

        var dto = new InventoryDto { StockQuantity = 15 };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetById(productId)).ReturnsAsync(product);

        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.DecreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(400, result.statusCode);
        Assert.Null(result.product);

        repositoryMock.Verify(r => r.DecreaseStock(productId, dto.StockQuantity), Times.Never);
    }

    [Fact]
    public async Task DecreaseStockAsync_WhenQuantityIsValid_ReturnsNoContent()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Laptop", "Test Laptop", 50000m, 10, Guid.NewGuid());
        var dto = new InventoryDto { StockQuantity = 3 };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetById(productId)).ReturnsAsync(product);
        repositoryMock
            .Setup(r => r.DecreaseStock(productId, dto.StockQuantity))
            .ReturnsAsync(product);
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        var result = await service.DecreaseStockAsync(productId, dto);

        Assert.Equal(204, result.statusCode);
        Assert.Equal(product, result.product);

        repositoryMock.Verify(r => r.DecreaseStock(productId, dto.StockQuantity), Times.Once);
    }

    [Fact]
    public async Task DecreaseStockAsync_WhenQuantityIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var dto = new InventoryDto { StockQuantity = 0 };
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var repositoryMock = new Mock<IProductRepository>();
        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.DecreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(400, result.statusCode);
        Assert.Null(result.product);

        repositoryMock.Verify(r => r.DecreaseStock(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DecreaseStockAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var dto = new InventoryDto { StockQuantity = 5 };

        var repositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        repositoryMock.Setup(r => r.GetById(productId)).ReturnsAsync((Product?)null);
        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.DecreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(404, result.statusCode);
        Assert.Null(result.product);

        repositoryMock.Verify(r => r.DecreaseStock(productId, dto.StockQuantity), Times.Never);
    }

    [Fact]
    public async Task IncreaseStockAsync_WhenQuantityIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var dto = new InventoryDto { StockQuantity = 0 };
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var repositoryMock = new Mock<IProductRepository>();
        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.IncreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(400, result.statusCode);
        Assert.Null(result.product);

        repositoryMock.Verify(r => r.IncreaseStock(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task IncreaseStockAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var dto = new InventoryDto { StockQuantity = 5 };

        var repositoryMock = new Mock<IProductRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        repositoryMock.Setup(r => r.GetById(productId)).ReturnsAsync((Product?)null);
        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.IncreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(404, result.statusCode);
        Assert.Null(result.product);

        repositoryMock.Verify(r => r.IncreaseStock(productId, dto.StockQuantity), Times.Never);
    }

    [Fact]
    public async Task IncreaseStockAsync_WhenValid_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product("Laptop", "Test laptop", 50000m, 10, Guid.NewGuid());

        var dto = new InventoryDto { StockQuantity = 5 };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetById(productId)).ReturnsAsync(product);

        repositoryMock
            .Setup(r => r.IncreaseStock(productId, dto.StockQuantity))
            .ReturnsAsync(product);

        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.IncreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(204, result.statusCode);

        repositoryMock.Verify(r => r.IncreaseStock(productId, dto.StockQuantity), Times.Once);
    }

    [Fact]
    public async Task IncreaseStockAsync_WhenRepositoryFails_ReturnsServerError()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product("Laptop", "Test laptop", 50000m, 10, Guid.NewGuid());

        var dto = new InventoryDto { StockQuantity = 5 };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetById(productId)).ReturnsAsync(product);

        repositoryMock
            .Setup(r => r.IncreaseStock(productId, dto.StockQuantity))
            .ReturnsAsync((Product?)null);

        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

        // Act
        var result = await service.IncreaseStockAsync(productId, dto);

        // Assert
        Assert.Equal(500, result.statusCode);
        Assert.Null(result.product);
    }

    [Fact]
public async Task GetProductsAsync_WhenProductsExist_ReturnsMappedResponse()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var products = new List<Product>
    {
        new Product(
            "Laptop",
            "Test laptop",
            50000m,
            10,
            categoryId
        ),
        new Product(
            "Mouse",
            "Test mouse",
            1000m,
            20,
            categoryId
        )
    };

    var queryParams = new ProductQueryParameters
    {
        PageNumber = 1,
        PageSize = 10
    };

    var repositoryMock = new Mock<IProductRepository>();
    var categoryRepositoryMock = new Mock<ICategoryRepository>();

    repositoryMock
        .Setup(r => r.GetProducts(queryParams))
        .ReturnsAsync((products.AsEnumerable(), 2));

    categoryRepositoryMock
        .Setup(c => c.GetById(It.IsAny<Guid>()))
        .ReturnsAsync((Category?)null);

    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.GetProductsAsync(queryParams);
    var resultList = result.Items.ToList(); // Convert to list for indexing

    // Assert
    Assert.Equal(2, result.TotalCount);
    Assert.Equal(1, result.PageNumber);
    Assert.Equal(10, result.PageSize);

    Assert.Equal(2, resultList.Count);

    Assert.Equal("Laptop", resultList[0].Name);
    Assert.Equal(50000m, resultList[0].Price);
    Assert.Equal(categoryId, resultList[0].CategoryId);

    Assert.Equal("Mouse", resultList[1].Name);

    // the reasone above is giving error "Cannot apply indexing with [] to an expression of type 'IEnumerable<ProductDto>'" is bcause result.Items is of type IEnumerable<ProductDto>, which does not support indexing. To fix this, you can convert it to a list or array before accessing elements by index. For example, you can change the assertion to: 
}

[Fact]
public async Task AddProductAsync_WhenRepositorySucceeds_ReturnsCreated()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var dto = new ProductDto
    {
        Name = "Laptop",
        Description = "Test laptop",
        Price = 50000m,
        StockQuantity = 10,
        CategoryId = categoryId
    };

    var createdProduct = new Product(
        dto.Name,
        dto.Description,
        dto.Price!.Value,
        dto.StockQuantity!.Value,
        dto.CategoryId
    );

    var repositoryMock = new Mock<IProductRepository>();
    var categoryRepositoryMock = new Mock<ICategoryRepository>();

    repositoryMock
        .Setup(r => r.AddProduct(It.IsAny<Product>()))
        .ReturnsAsync(createdProduct);

    categoryRepositoryMock
        .Setup(c => c.GetById(categoryId))
        .ReturnsAsync(new Category("Test Category"));

    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.AddProductAsync(dto);

    // Assert
    Assert.Equal(201, result.statusCode);
    Assert.Equal(createdProduct, result.product);

    repositoryMock.Verify(
        r => r.AddProduct(It.IsAny<Product>()),
        Times.Once
    );
}

[Fact]
public async Task AddProductAsync_WhenRepositoryFails_ReturnsServerError()
{
    // Arrange
    var dto = new ProductDto
    {
        Name = "Laptop",
        Description = "Test laptop",
        Price = 50000m,
        StockQuantity = 10,
        CategoryId = Guid.NewGuid()
    };

    var repositoryMock = new Mock<IProductRepository>();
    var categoryRepositoryMock = new Mock<ICategoryRepository>();

    repositoryMock
        .Setup(r => r.AddProduct(It.IsAny<Product>()))
        .ReturnsAsync((Product?)null);

    categoryRepositoryMock
        .Setup(c => c.GetById(dto.CategoryId))
        .ReturnsAsync(new Category("Test Category"));

    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.AddProductAsync(dto);

    // Assert
    Assert.Equal(500, result.statusCode);
    Assert.Null(result.product);
}

[Fact]
public async Task UpdateProduct_WhenIdIsEmpty_ReturnsBadRequest()// how to update this test to check for empty id in the UpdateProduct method? 
{
    // Arrange
    var dto = new UpdateProductDto
    {
        Name = "Updated"
    };
    var categoryRepositoryMock = new Mock<ICategoryRepository>();

    var repositoryMock = new Mock<IProductRepository>();
    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.UpdateProduct(Guid.Empty, dto);

    // Assert
    Assert.Equal(400, result.statusCode);
    Assert.Null(result.product);

    repositoryMock.Verify(
        r => r.GetById(It.IsAny<Guid>()),
        Times.Never
    );
}

[Fact]
public async Task UpdateProduct_WhenProductDoesNotExist_ReturnsNotFound()
{
    // Arrange
    var id = Guid.NewGuid();

    var dto = new UpdateProductDto
    {
        Name = "Updated"
    };

    var repositoryMock = new Mock<IProductRepository>();

    repositoryMock
        .Setup(r => r.GetById(id))
        .ReturnsAsync((Product?)null);

    var categoryRepositoryMock = new Mock<ICategoryRepository>();
    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.UpdateProduct(id, dto);

    // Assert
    Assert.Equal(404, result.statusCode);
    Assert.Null(result.product);

    repositoryMock.Verify(
        r => r.UpdateProduct(It.IsAny<Product>()),
        Times.Never
    );
}

[Fact]
public async Task UpdateProduct_WhenProductExists_UpdatesProduct()
{
    // Arrange
    var id = Guid.NewGuid();
    var categoryId = Guid.NewGuid();

    var existingProduct = new Product(
        "Old Name",
        "Old description",
        100m,
        5,
        categoryId
    );

    // Important because Product constructor generates a new ID
    // existingProduct.Id = id;

    var dto = new UpdateProductDto
    {
        Name = "New Name",
        Description = "New description",
        Price = 200m,
        CategoryId = categoryId
    };

    var repositoryMock = new Mock<IProductRepository>();

    repositoryMock
        .Setup(r => r.GetById(id))
        .ReturnsAsync(existingProduct);

    repositoryMock
        .Setup(r => r.UpdateProduct(existingProduct))
        .ReturnsAsync(existingProduct);

    var categoryRepositoryMock = new Mock<ICategoryRepository>();
    categoryRepositoryMock
        .Setup(c => c.GetById(categoryId))
        .ReturnsAsync((Category?)null);

    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.UpdateProduct(id, dto);

    // Assert
    Assert.Equal(204, result.statusCode);

    Assert.Equal("New Name", existingProduct.Name);
    Assert.Equal("New description", existingProduct.Description);
    Assert.Equal(200m, existingProduct.Price);

    repositoryMock.Verify(
        r => r.UpdateProduct(existingProduct),
        Times.Once
    );
}

[Fact]
public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsNotFound() // why this test is not succeeding? 
{
    // Arrange
    var id = Guid.NewGuid();

    var repositoryMock = new Mock<IProductRepository>();

    repositoryMock
        .Setup(r => r.DeleteProduct(id))
        .ReturnsAsync(false);

    var categoryRepositoryMock = new Mock<ICategoryRepository>();

    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.DeleteAsync(id);

    // Assert
    Assert.Equal(404, result.statusCode);
    Assert.False(result.Success);
}

[Fact]
public async Task DeleteAsync_WhenProductExists_ReturnsSuccess()
{
    // Arrange
    var id = Guid.NewGuid();

    var repositoryMock = new Mock<IProductRepository>();

    repositoryMock
        .Setup(r => r.DeleteProduct(id))
        .ReturnsAsync(true);

    var categoryRepositoryMock = new Mock<ICategoryRepository>();

    var service = new ProductService(repositoryMock.Object, categoryRepositoryMock.Object);

    // Act
    var result = await service.DeleteAsync(id);

    // Assert
    Assert.Equal(200, result.statusCode);
    Assert.True(result.Success);

    repositoryMock.Verify(
        r => r.DeleteProduct(id),
        Times.Once
    );
}
}
