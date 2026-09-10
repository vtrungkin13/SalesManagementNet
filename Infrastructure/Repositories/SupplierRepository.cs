using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SupplierRepository(AppDbContext db) : ISupplierRepository
{
    public async Task<IReadOnlyList<Supplier>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.Suppliers.Where(x => x.TenantId == tenantId).AsNoTracking().ToListAsync(cancellationToken);

    public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        db.Suppliers.Add(supplier);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

