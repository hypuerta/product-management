using Moq;
using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Domain;

namespace ProductManagement.Application.Tests;

public class AuthServiceTests
{
    [Theory]
    [InlineData("", "password")]
    [InlineData("   ", "password")]
    [InlineData("user@example.com", "")]
    [InlineData("user@example.com", "   ")]
    public async Task RegisterAsync_returns_false_for_invalid_email_or_password(string email, string password)
    {
        var repository = new Mock<IUserRepository>();
        var passwords = new Mock<IPasswordService>();
        var tokens = new Mock<ITokenService>();
        var service = new AuthService(repository.Object, passwords.Object, tokens.Object);

        var result = await service.RegisterAsync(new RegisterRequest(email, password), CancellationToken.None);

        Assert.False(result);
        repository.Verify(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        passwords.Verify(p => p.Hash(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_returns_false_when_email_already_exists()
    {
        var repository = new Mock<IUserRepository>();
        var passwords = new Mock<IPasswordService>();
        var tokens = new Mock<ITokenService>();
        var service = new AuthService(repository.Object, passwords.Object, tokens.Object);

        var existing = new User("user@example.com", "existingHash");

        repository
            .Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await service.RegisterAsync(
            new RegisterRequest("  USER@EXAMPLE.COM  ", "secret"),
            CancellationToken.None);

        Assert.False(result);
        repository.Verify(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        passwords.Verify(p => p.Hash(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_adds_user_and_saves_when_request_is_valid_and_email_is_new()
    {
        var repository = new Mock<IUserRepository>();
        var passwords = new Mock<IPasswordService>();
        var tokens = new Mock<ITokenService>();
        var service = new AuthService(repository.Object, passwords.Object, tokens.Object);

        repository
            .Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        passwords
            .Setup(p => p.Hash("secret"))
            .Returns("hashed-secret");

        User? addedUser = null;
        repository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => addedUser = u)
            .Returns(Task.CompletedTask);

        var result = await service.RegisterAsync(
            new RegisterRequest("  USER@EXAMPLE.COM  ", "secret"),
            CancellationToken.None);

        Assert.True(result);
        Assert.NotNull(addedUser);
        Assert.Equal("user@example.com", addedUser!.Email);
        Assert.Equal("hashed-secret", addedUser.PasswordHash);

        passwords.Verify(p => p.Hash("secret"), Times.Once);
        repository.Verify(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_returns_null_when_user_is_not_found()
    {
        var repository = new Mock<IUserRepository>();
        var passwords = new Mock<IPasswordService>();
        var tokens = new Mock<ITokenService>();
        var service = new AuthService(repository.Object, passwords.Object, tokens.Object);

        repository
            .Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await service.LoginAsync(
            new LoginRequest("  USER@EXAMPLE.COM  ", "secret"),
            CancellationToken.None);

        Assert.Null(result);
        repository.Verify(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
        passwords.Verify(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        tokens.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_returns_null_when_password_verification_fails()
    {
        var repository = new Mock<IUserRepository>();
        var passwords = new Mock<IPasswordService>();
        var tokens = new Mock<ITokenService>();
        var service = new AuthService(repository.Object, passwords.Object, tokens.Object);

        var user = new User("user@example.com", "stored-hash");

        repository
            .Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwords
            .Setup(p => p.Verify("wrong-password", "stored-hash"))
            .Returns(false);

        var result = await service.LoginAsync(
            new LoginRequest("user@example.com", "wrong-password"),
            CancellationToken.None);

        Assert.Null(result);
        passwords.Verify(p => p.Verify("wrong-password", "stored-hash"), Times.Once);
        tokens.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_returns_auth_response_when_credentials_are_valid()
    {
        var repository = new Mock<IUserRepository>();
        var passwords = new Mock<IPasswordService>();
        var tokens = new Mock<ITokenService>();
        var service = new AuthService(repository.Object, passwords.Object, tokens.Object);

        var user = new User("user@example.com", "stored-hash");

        repository
            .Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwords
            .Setup(p => p.Verify("secret", "stored-hash"))
            .Returns(true);

        tokens
            .Setup(t => t.CreateToken(user))
            .Returns("jwt-token");

        var result = await service.LoginAsync(
            new LoginRequest("  USER@EXAMPLE.COM  ", "secret"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result!.Token);
        Assert.Equal("user@example.com", result.Email);

        repository.Verify(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()), Times.Once);
        passwords.Verify(p => p.Verify("secret", "stored-hash"), Times.Once);
        tokens.Verify(t => t.CreateToken(user), Times.Once);
    }
}