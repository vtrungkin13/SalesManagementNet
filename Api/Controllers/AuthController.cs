using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public Task<TokenResponse> Register(
        RegisterRequest request,
        CancellationToken cancellationToken) =>
        auth.RegisterAsync(request, cancellationToken);

    [HttpPost("login")]
    public Task<TokenResponse> Login(
        LoginRequest request,
        CancellationToken cancellationToken) =>
        auth.LoginAsync(request, cancellationToken);

    [HttpPost("refresh")]
    public Task<TokenResponse> Refresh(
        RefreshRequest request,
        CancellationToken cancellationToken) =>
        auth.RefreshAsync(request.RefreshToken, cancellationToken);

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        await auth.RevokeAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }
}
