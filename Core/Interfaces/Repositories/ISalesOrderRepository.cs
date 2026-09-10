using Core.Entities;

namespace Core.Interfaces;

public interface ISalesOrderRepository
{
    Task<IReadOnlyList<SalesOrder>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> CustomerExistsAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<bool> WarehouseExistsAsync(Guid tenantId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<SalesOrder?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<Inventory?> GetInventoryAsync(Guid tenantId, Guid warehouseId, Guid variantId, CancellationToken cancellationToken = default);
    Task AddAsync(SalesOrder order, CancellationToken cancellationToken = default);
    Task AddInventoryTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
