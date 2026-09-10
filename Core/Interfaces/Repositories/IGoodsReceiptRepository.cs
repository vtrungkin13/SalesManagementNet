using Core.Entities;

namespace Core.Interfaces;

public interface IGoodsReceiptRepository
{
    Task<bool> PurchaseOrderExistsAsync(Guid tenantId, Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<bool> WarehouseExistsAsync(Guid tenantId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<bool> VariantsBelongToTenantAsync(Guid tenantId, IEnumerable<Guid> variantIds, CancellationToken cancellationToken = default);
    Task<GoodsReceipt?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetPurchaseOrderAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<Inventory?> GetInventoryAsync(Guid tenantId, Guid warehouseId, Guid variantId, CancellationToken cancellationToken = default);
    Task AddAsync(GoodsReceipt receipt, CancellationToken cancellationToken = default);
    Task AddInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task AddInventoryTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
