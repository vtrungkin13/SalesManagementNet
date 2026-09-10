using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GoodsReceiptRepository(AppDbContext db) : IGoodsReceiptRepository
{
    public Task<bool> PurchaseOrderExistsAsync(Guid tenantId, Guid purchaseOrderId, CancellationToken cancellationToken = default) =>
        db.PurchaseOrders.AnyAsync(x => x.TenantId == tenantId && x.Id == purchaseOrderId, cancellationToken);

    public Task<bool> WarehouseExistsAsync(Guid tenantId, Guid warehouseId, CancellationToken cancellationToken = default) =>
        db.Warehouses.AnyAsync(x => x.TenantId == tenantId && x.Id == warehouseId, cancellationToken);

    public Task<bool> VariantsBelongToTenantAsync(Guid tenantId, IEnumerable<Guid> variantIds, CancellationToken cancellationToken = default)
    {
        var ids = variantIds.Distinct().ToList();
        return db.ProductVariants.CountAsync(x => x.TenantId == tenantId && ids.Contains(x.Id), cancellationToken)
            .ContinueWith(t => t.Result == ids.Count, cancellationToken);
    }

    public Task<GoodsReceipt?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default) =>
        db.GoodsReceipts.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public Task<PurchaseOrder?> GetPurchaseOrderAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default) =>
        db.PurchaseOrders.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public Task<Inventory?> GetInventoryAsync(Guid tenantId, Guid warehouseId, Guid variantId, CancellationToken cancellationToken = default) =>
        db.Inventories.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.WarehouseId == warehouseId && x.ProductVariantId == variantId, cancellationToken);

    public Task AddAsync(GoodsReceipt receipt, CancellationToken cancellationToken = default)
    {
        db.GoodsReceipts.Add(receipt);
        return Task.CompletedTask;
    }

    public Task AddInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        db.Inventories.Add(inventory);
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
