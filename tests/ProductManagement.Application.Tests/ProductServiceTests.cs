using Moq;
using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Domain;

namespace ProductManagement.Application.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_maps_repository_products_to_dtos()
    {
        var repository = new Mock<IProductRepository>();
        var p1 = new Product("Keyboard", 49.99m, 4);
        var p2 = new Product("Mouse", 19.99m, 10);

        repository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { p1, p2 });

        var service = new ProductService(repository.Object);

        var result = await service.GetAllAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(p1.Id, result[0].Id);
        Assert.Equal("Keyboard", result[0].Name);
        Assert.Equal(49.99m, result[0].Price);
        Assert.Equal(4, result[0].Quantity);

        Assert.Equal(p2.Id, result[1].Id);
        Assert.Equal("Mouse", result[1].Name);
        Assert.Equal(19.99m, result[1].Price);
        Assert.Equal(10, result[1].Quantity);

        repository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_returns_mapped_dto_when_product_exists()
    {
        var repository = new Mock<IProductRepository>();
        var product = new Product("Monitor", 199.99m, 3);

        repository
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var service = new ProductService(repository.Object);

        var result = await service.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
        Assert.Equal("Monitor", result.Name);
        Assert.Equal(199.99m, result.Price);
        Assert.Equal(3, result.Quantity);

        repository.Verify(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_product_does_not_exist()
    {
        var repository = new Mock<IProductRepository>();
        var id = Guid.NewGuid();

        repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(repository.Object);

        var result = await service.GetByIdAsync(id, CancellationToken.None);

        Assert.Null(result);
        repository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_creates_a_valid_product()
    {
        var repository = new Mock<IProductRepository>();
        var service = new ProductService(repository.Object);

        var result = await service.CreateAsync(
            new CreateProductRequest("Keyboard", 49.99m, 4),
            CancellationToken.None);

        Assert.Equal("Keyboard", result.Name);
        Assert.Equal(49.99m, result.Price);
        Assert.Equal(4, result.Quantity);

        repository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_updates_product_and_returns_dto_when_product_exists()
    {
        var repository = new Mock<IProductRepository>();
        var product = new Product("Old Name", 10m, 1);
        var request = new UpdateProductRequest("New Name", 20m, 5);

        repository
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var service = new ProductService(repository.Object);

        var result = await service.UpdateAsync(product.Id, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(20m, result.Price);
        Assert.Equal(5, result.Quantity);

        Assert.Equal("New Name", product.Name);
        Assert.Equal(20m, product.Price);
        Assert.Equal(5, product.Quantity);

        repository.Verify(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_returns_null_and_does_not_save_when_product_does_not_exist()
    {
        var repository = new Mock<IProductRepository>();
        var id = Guid.NewGuid();
        var request = new UpdateProductRequest("New Name", 20m, 5);

        repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(repository.Object);

        var result = await service.UpdateAsync(id, request, CancellationToken.None);

        Assert.Null(result);
        repository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_deletes_product_and_returns_true_when_product_exists()
    {
        var repository = new Mock<IProductRepository>();
        var product = new Product("Keyboard", 49.99m, 4);

        repository
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var service = new ProductService(repository.Object);

        var result = await service.DeleteAsync(product.Id, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_returns_false_and_does_not_delete_when_product_does_not_exist()
    {
        var repository = new Mock<IProductRepository>();
        var id = Guid.NewGuid();

        repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(repository.Object);

        var result = await service.DeleteAsync(id, CancellationToken.None);

        Assert.False(result);
        repository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}