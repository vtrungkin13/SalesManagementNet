using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppUser>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(AppUser user, CancellationToken cancellationToken = default);
    Task AddRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

