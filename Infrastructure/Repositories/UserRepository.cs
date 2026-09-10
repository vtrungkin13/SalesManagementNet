using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<AppUser?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.AppUsers.Include(x => x.UserRoles).ThenInclude(x => x.Role).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<AppUser>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.AppUsers.Where(x => x.TenantId == tenantId).Include(x => x.UserRoles).ThenInclude(x => x.Role).AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.AppUsers.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default) =>
        db.Roles.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

    public Task AddAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        db.AppUsers.Add(user);
        return Task.CompletedTask;
    }

    public Task AddRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        db.UserRoles.Add(userRole);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

