using Core.Entities;

namespace Core.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<IReadOnlyList<PurchaseOrder>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> SupplierExistsAsync(Guid tenantId, Guid supplierId, CancellationToken cancellationToken = default);
    Task<bool> WarehouseExistsAsync(Guid tenantId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<bool> VariantsBelongToTenantAsync(Guid tenantId, IEnumerable<Guid> variantIds, CancellationToken cancellationToken = default);
    Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

