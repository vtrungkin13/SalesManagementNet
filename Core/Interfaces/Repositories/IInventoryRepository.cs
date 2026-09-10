using Core.Entities;

namespace Core.Interfaces;

public interface IInventoryRepository
{
    Task<IReadOnlyList<Inventory>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Inventory?> GetAsync(Guid tenantId, Guid warehouseId, Guid productVariantId, CancellationToken cancellationToken = default);
    Task<ProductVariant?> GetVariantAsync(Guid tenantId, Guid productVariantId, CancellationToken cancellationToken = default);
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
