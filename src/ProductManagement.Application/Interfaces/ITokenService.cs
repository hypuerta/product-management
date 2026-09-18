using ProductManagement.Domain;

namespace ProductManagement.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
