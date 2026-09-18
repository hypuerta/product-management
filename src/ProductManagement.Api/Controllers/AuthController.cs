using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;

namespace ProductManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var created = await auth.RegisterAsync(request, cancellationToken);
        return created ? StatusCode(StatusCodes.Status201Created) : Conflict(new { message = "A valid, unique email and password are required." });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await auth.LoginAsync(request, cancellationToken);
        return response is null ? Unauthorized() : Ok(response);
    }
}
