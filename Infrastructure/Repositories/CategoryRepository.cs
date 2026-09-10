using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.Categories.Where(x => x.TenantId == tenantId).AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> ExistsByCodeAsync(Guid tenantId, string code, CancellationToken cancellationToken = default) =>
        db.Categories.AnyAsync(x => x.TenantId == tenantId && x.Code == code, cancellationToken);

    public Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        db.Categories.Add(category);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

