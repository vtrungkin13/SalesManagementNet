using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SalesManagement.Api.Entities;

namespace SalesManagement.Api.Security
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _configuration["Jwt:SecretKey"] ?? "aGVsbG93b3JsZHRoaXNpc2FzdXBlcnN1cGVydmVyeWxvbmdhbmRzdHJvbmdzZWNyZXRrZXlhbmRkb250dGVsbG1ldGhhdHRoaXNpc25vdGxvbmdlbm91Z2hvcnRvb3Nob3J0";
            
            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(jwtSecret);
            }
            catch
            {
                keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
            }
            
            var key = new SymmetricSecurityKey(keyBytes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("tenant_id", user.TenantId.ToString())
            };

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var expirationMs = double.Parse(_configuration["Jwt:ExpirationMs"] ?? "900000"); // 15 mins
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMilliseconds(expirationMs),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
