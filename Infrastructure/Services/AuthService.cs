using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public sealed class AuthService(IAuthRepository repository, IConfiguration config) : IAuthService
{
    public async Task<TokenResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        if (await repository.UserExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var tenant = await repository.GetTenantByCodeAsync(request.TenantCode, cancellationToken);
        if (tenant is null)
        {
            tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Code = request.TenantCode,
                Name = request.TenantName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddTenantAsync(tenant, cancellationToken);
        }

        var role = await repository.GetRoleByNameAsync("ADMIN", cancellationToken);
        if (role is null)
        {
            role = new Role { Id = Guid.NewGuid(), Name = "ADMIN" };
            await repository.AddRoleAsync(role, cancellationToken);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role
        });

        await repository.AddUserAsync(user, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return await IssueAsync(user, cancellationToken);
    }

    public async Task<TokenResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByEmailAsync(
            NormalizeEmail(request.Email), cancellationToken);

        if (user is null || user.Status != UserStatus.ACTIVE ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await IssueAsync(user, cancellationToken);
    }

    public async Task<TokenResponse> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var current = await repository.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (current is null || !current.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        current.RevokedAt = DateTime.UtcNow;

        var response = await IssueAsync(current.User, cancellationToken);
        current.ReplacedByToken = response.RefreshToken;
        await repository.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var current = await repository.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (current is null)
        {
            return;
        }

        current.RevokedAt ??= DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<TokenResponse> IssueAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var accessExpiresAt = now.AddMinutes(GetAccessTokenMinutes());
        var refreshExpiresAt = now.AddDays(GetRefreshTokenDays());
        var refreshToken = GenerateRefreshToken();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.UserRoles.Select(x => x.Role.Name).Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("tenantId", user.TenantId.Value.ToString()));
        }

        var key = config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is required.");
        var issuer = config["Jwt:Issuer"] ?? "SalesManagement";
        var audience = config["Jwt:Audience"] ?? "SalesManagement";

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: accessExpiresAt,
            signingCredentials: credentials);

        await repository.AddRefreshTokenAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = now
        }, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(jwt),
            refreshToken,
            accessExpiresAt,
            refreshExpiresAt);
    }

    private int GetAccessTokenMinutes() =>
        int.TryParse(config["Jwt:AccessTokenMinutes"], out var value) ? value : 15;

    private int GetRefreshTokenDays() =>
        int.TryParse(config["Jwt:RefreshTokenDays"], out var value) ? value : 7;

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
