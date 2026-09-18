using Moq;
using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;

namespace ProductManagement.Application.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_creates_a_valid_product()
    {
        var repository = new Mock<IProductRepository>();
        var service = new ProductService(repository.Object);

        var result = await service.CreateAsync(new CreateProductRequest("Keyboard", 49.99m, 4), CancellationToken.None);

        Assert.Equal("Keyboard", result.Name);
        Assert.Equal(49.99m, result.Price);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}