using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class WarehouseRepository(AppDbContext db) : IWarehouseRepository
{
    public async Task<IReadOnlyList<Warehouse>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.Warehouses.Where(x => x.TenantId == tenantId).AsNoTracking().ToListAsync(cancellationToken);

    public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        db.Warehouses.Add(warehouse);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

