using Core.Entities;

namespace Core.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

