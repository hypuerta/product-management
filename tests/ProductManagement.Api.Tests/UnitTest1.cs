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
}