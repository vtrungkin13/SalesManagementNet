using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SalesOrderRepository(AppDbContext db) : ISalesOrderRepository
{
    public async Task<IReadOnlyList<SalesOrder>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.SalesOrders.Where(x => x.TenantId == tenantId).Include(x => x.Customer).Include(x => x.Warehouse).Include(x => x.Items).ThenInclude(x => x.ProductVariant).AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> CustomerExistsAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) =>
        db.Customers.AnyAsync(x => x.TenantId == tenantId && x.Id == customerId, cancellationToken);

    public Task<bool> WarehouseExistsAsync(Guid tenantId, Guid warehouseId, CancellationToken cancellationToken = default) =>
        db.Warehouses.AnyAsync(x => x.TenantId == tenantId && x.Id == warehouseId, cancellationToken);

    public Task<SalesOrder?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default) =>
        db.SalesOrders.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public Task<Inventory?> GetInventoryAsync(Guid tenantId, Guid warehouseId, Guid variantId, CancellationToken cancellationToken = default) =>
        db.Inventories.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.WarehouseId == warehouseId && x.ProductVariantId == variantId, cancellationToken);

    public Task AddAsync(SalesOrder order, CancellationToken cancellationToken = default)
    {
        db.SalesOrders.Add(order);
        return Task.CompletedTask;
    }

    public Task AddInventoryTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
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
