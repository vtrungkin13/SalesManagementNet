using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Api.Data;
using SalesManagement.Api.Dtos;
using SalesManagement.Api.Entities;
using SalesManagement.Api.Exceptions;
using SalesManagement.Api.Security;

namespace SalesManagement.Api.Services
{
    public class AuthService
    {
        private readonly SalesDbContext _db;
        private readonly TokenService _tokenService;

        public AuthService(SalesDbContext db, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users
                .Include(u => u.Roles)
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new InvalidOperationException("Email hoặc mật khẩu không chính xác.");

            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = await CreateRefreshTokenAsync(user);

            return new AuthResponse(accessToken, refreshToken.Token);
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                throw new EmailAlreadyExistsException(request.Email);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var defaultRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "USER");
            var roles = defaultRole != null ? new List<Role> { defaultRole } : new List<Role>();

            var user = new AppUser
            {
                Email = request.Email,
                Password = hashedPassword,
                Name = request.Name,
                Phone = request.Phone,
                Roles = roles
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new RegisterResponse(user.Id, user.Email, user.Name, user.Tenant?.Code ?? "");
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token)
        {
            var refreshToken = await _db.RefreshTokens
                .Include(r => r.AppUser).ThenInclude(u => u!.Roles)
                .Include(r => r.AppUser).ThenInclude(u => u!.Tenant)
                .FirstOrDefaultAsync(r => r.Token == token)
                ?? throw new InvalidRefreshTokenException("Refresh token không tồn tại");

            if (refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                _db.RefreshTokens.RemoveRange(_db.RefreshTokens.Where(r => r.UserId == refreshToken.UserId));
                await _db.SaveChangesAsync();
                throw new InvalidRefreshTokenException("Refresh token đã hết hạn. Vui lòng đăng nhập lại");
            }

            _db.RefreshTokens.Remove(refreshToken);
            var newRefreshToken = await CreateRefreshTokenAsync(refreshToken.AppUser!);
            var accessToken = _tokenService.GenerateToken(refreshToken.AppUser!);

            return new AuthResponse(accessToken, newRefreshToken.Token);
        }

        public async Task LogoutAsync(string token)
        {
            var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token)
                ?? throw new InvalidRefreshTokenException("Refresh token không tồn tại");

            _db.RefreshTokens.RemoveRange(_db.RefreshTokens.Where(r => r.UserId == refreshToken.UserId));
            await _db.SaveChangesAsync();
        }

        private async Task<RefreshToken> CreateRefreshTokenAsync(AppUser user)
        {
            var rt = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();
            return rt;
        }
    }
}
