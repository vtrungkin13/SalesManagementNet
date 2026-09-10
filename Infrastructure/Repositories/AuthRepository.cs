using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AuthRepository(AppDbContext db) : IAuthRepository
{
    public Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.AppUsers.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<AppUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.AppUsers.Include(x => x.UserRoles).ThenInclude(x => x.Role).SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<Tenant?> GetTenantByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        db.Tenants.SingleOrDefaultAsync(x => x.Code == code, cancellationToken);

    public Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default) =>
        db.Roles.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

    public Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default) =>
        db.RefreshTokens.Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Token == token, cancellationToken);

    public Task AddTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        db.Tenants.Add(tenant);
        return Task.CompletedTask;
    }

    public Task AddRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        db.Roles.Add(role);
        return Task.CompletedTask;
    }

    public Task AddUserAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        db.AppUsers.Add(user);
        return Task.CompletedTask;
    }

    public Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        db.RefreshTokens.Add(token);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

