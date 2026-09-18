using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain;

namespace ProductManagement.Application.Services;

public sealed class AuthService(IUserRepository repository, IPasswordService passwords, ITokenService tokens) : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password)) return false;
        var email = request.Email.Trim().ToLowerInvariant();
        if (await repository.GetByEmailAsync(email, cancellationToken) is not null) return false;
        await repository.AddAsync(new User(email, passwords.Hash(request.Password)), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        return user is not null && passwords.Verify(request.Password, user.PasswordHash)
            ? new AuthResponse(tokens.CreateToken(user), user.Email)
            : null;
    }
}
