using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Api.Controllers;
using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;

namespace ProductManagement.Api.Tests;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetById_returns_not_found_when_product_is_missing()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_returns_created_at_action_when_product_is_created()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var request = new CreateProductRequest("Keyboard", 99.99m, 5);
        var createdProduct = new ProductDto(Guid.NewGuid(), "Keyboard", 99.99m, 5);
        var cancellationToken = CancellationToken.None;

        service
            .Setup(s => s.CreateAsync(request, cancellationToken))
            .ReturnsAsync(createdProduct);

        var result = await controller.Create(request, cancellationToken);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProductsController.GetById), created.ActionName);
        Assert.Equal(createdProduct, created.Value);
        Assert.NotNull(created.RouteValues);
        Assert.Equal(createdProduct.Id, created.RouteValues!["id"]);

        service.Verify(s => s.CreateAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_service_throws_argument_exception()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var request = new CreateProductRequest("", -1m, -5);
        var cancellationToken = CancellationToken.None;
        const string errorMessage = "Invalid product data.";

        service
            .Setup(s => s.CreateAsync(request, cancellationToken))
            .ThrowsAsync(new ArgumentException(errorMessage));

        var result = await controller.Create(request, cancellationToken);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequest.Value);

        var messageProperty = badRequest.Value!.GetType().GetProperty("message");
        Assert.NotNull(messageProperty);
        Assert.Equal(errorMessage, messageProperty!.GetValue(badRequest.Value));

        service.Verify(s => s.CreateAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Update_returns_ok_when_product_is_updated()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var id = Guid.NewGuid();
        var request = new UpdateProductRequest("Updated Keyboard", 129.99m, 7);
        var updatedProduct = new ProductDto(id, "Updated Keyboard", 129.99m, 7);
        var cancellationToken = CancellationToken.None;

        service
            .Setup(s => s.UpdateAsync(id, request, cancellationToken))
            .ReturnsAsync(updatedProduct);

        var result = await controller.Update(id, request, cancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(updatedProduct, ok.Value);
        service.Verify(s => s.UpdateAsync(id, request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Update_returns_not_found_when_product_does_not_exist()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var id = Guid.NewGuid();
        var request = new UpdateProductRequest("Updated Keyboard", 129.99m, 7);
        var cancellationToken = CancellationToken.None;

        service
            .Setup(s => s.UpdateAsync(id, request, cancellationToken))
            .ReturnsAsync((ProductDto?)null);

        var result = await controller.Update(id, request, cancellationToken);

        Assert.IsType<NotFoundResult>(result.Result);
        service.Verify(s => s.UpdateAsync(id, request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Delete_returns_no_content_when_product_is_deleted()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        service
            .Setup(s => s.DeleteAsync(id, cancellationToken))
            .ReturnsAsync(true);

        var result = await controller.Delete(id, cancellationToken);

        Assert.IsType<NoContentResult>(result);
        service.Verify(s => s.DeleteAsync(id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Delete_returns_not_found_when_product_does_not_exist()
    {
        var service = new Mock<IProductService>();
        var controller = new ProductsController(service.Object);

        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        service
            .Setup(s => s.DeleteAsync(id, cancellationToken))
            .ReturnsAsync(false);

        var result = await controller.Delete(id, cancellationToken);

        Assert.IsType<NotFoundResult>(result);
        service.Verify(s => s.DeleteAsync(id, cancellationToken), Times.Once);
    }
}