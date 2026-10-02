using System;
using InventoryManagement.Api.Interfaces;
using InventoryManagement.Api.Services;
using Moq;

namespace InventoryManagement.Api.Tests.Services;

public class CategoryServiceTests
{

    [Fact]
public async Task DeleteAsync_WhenCategoryHasProducts_Returns500()
{
    // Arrange
    var id = Guid.NewGuid();

    var repositoryMock = new Mock<ICategoryRepository>();
    repositoryMock
        .Setup(r => r.HasProducts(id))
        .ReturnsAsync(true);

    var service = new CategoryService(repositoryMock.Object);

    // Act
    var result = await service.DeleteAsync(id);

    // Assert
    Assert.Equal(409, result.statusCode);
    Assert.Equal(
        "Cannot delete category because it is associated with existing products.",
        result.message
    );

    repositoryMock.Verify(
        r => r.DeleteCategory(It.IsAny<Guid>()),
        Times.Never
    );
}

[Fact]
public async Task DeleteAsync_WhenCategoryHasNoProducts_DeletesCategory()
{
    // Arrange
    var id = Guid.NewGuid();

    var repositoryMock = new Mock<ICategoryRepository>();

    repositoryMock
        .Setup(r => r.HasProducts(id))
        .ReturnsAsync(false);

    repositoryMock
        .Setup(r => r.DeleteCategory(id))
        .ReturnsAsync(true);

    var service = new CategoryService(repositoryMock.Object);

    // Act
    var result = await service.DeleteAsync(id);

    // Assert
    Assert.Equal(201, result.statusCode);
    Assert.Equal("The category was deleted successfully!", result.message);

    repositoryMock.Verify(
        r => r.DeleteCategory(id),
        Times.Once
    );
}

}
