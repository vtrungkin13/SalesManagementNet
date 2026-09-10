using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InventoryRepository(AppDbContext db) : IInventoryRepository
{
    public async Task<IReadOnlyList<Inventory>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.Inventories.Where(x => x.TenantId == tenantId).Include(x => x.ProductVariant).Include(x => x.Warehouse).AsNoTracking().ToListAsync(cancellationToken);

    public Task<Inventory?> GetAsync(Guid tenantId, Guid warehouseId, Guid productVariantId, CancellationToken cancellationToken = default) =>
        db.Inventories.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.WarehouseId == warehouseId && x.ProductVariantId == productVariantId, cancellationToken);

    public Task<ProductVariant?> GetVariantAsync(Guid tenantId, Guid productVariantId, CancellationToken cancellationToken = default) =>
        db.ProductVariants.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == productVariantId, cancellationToken);

    public Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        db.Inventories.Add(inventory);
        return Task.CompletedTask;
    }

    public Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
    {
        db.InventoryTransactions.Add(transaction);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var result = await action();
        await transaction.CommitAsync(cancellationToken);
        return result;
    }
}
