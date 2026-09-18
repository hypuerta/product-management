using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.Api.Controllers;
using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;

namespace ProductManagement.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_returns_201_created_when_registration_succeeds()
    {
        var auth = new Mock<IAuthService>();
        var controller = new AuthController(auth.Object);

        var request = new RegisterRequest("user@example.com", "Pass123!");
        var cancellationToken = CancellationToken.None;

        auth
            .Setup(a => a.RegisterAsync(request, cancellationToken))
            .ReturnsAsync(true);

        var result = await controller.Register(request, cancellationToken);

        var created = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        auth.Verify(a => a.RegisterAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Register_returns_conflict_when_registration_fails()
    {
        var auth = new Mock<IAuthService>();
        var controller = new AuthController(auth.Object);

        var request = new RegisterRequest("duplicate@example.com", "Pass123!");
        var cancellationToken = CancellationToken.None;

        auth
            .Setup(a => a.RegisterAsync(request, cancellationToken))
            .ReturnsAsync(false);

        var result = await controller.Register(request, cancellationToken);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);

        var messageProperty = conflict.Value?.GetType().GetProperty("message");
        Assert.NotNull(messageProperty);
        Assert.Equal(
            "A valid, unique email and password are required.",
            messageProperty!.GetValue(conflict.Value)
        );

        auth.Verify(a => a.RegisterAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Login_returns_ok_with_auth_response_when_credentials_are_valid()
    {
        var auth = new Mock<IAuthService>();
        var controller = new AuthController(auth.Object);

        var request = new LoginRequest("user@example.com", "Pass123!");
        var expected = new AuthResponse("token-value", "user@example.com");
        var cancellationToken = CancellationToken.None;

        auth
            .Setup(a => a.LoginAsync(request, cancellationToken))
            .ReturnsAsync(expected);

        var result = await controller.Login(request, cancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal(expected, payload);

        auth.Verify(a => a.LoginAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Login_returns_unauthorized_when_credentials_are_invalid()
    {
        var auth = new Mock<IAuthService>();
        var controller = new AuthController(auth.Object);

        var request = new LoginRequest("user@example.com", "wrong-password");
        var cancellationToken = CancellationToken.None;

        auth
            .Setup(a => a.LoginAsync(request, cancellationToken))
            .ReturnsAsync((AuthResponse?)null);

        var result = await controller.Login(request, cancellationToken);

        Assert.IsType<UnauthorizedResult>(result.Result);
        auth.Verify(a => a.LoginAsync(request, cancellationToken), Times.Once);
    }
}