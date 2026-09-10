using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PurchaseOrderRepository(AppDbContext db) : IPurchaseOrderRepository
{
    public async Task<IReadOnlyList<PurchaseOrder>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.PurchaseOrders.Where(x => x.TenantId == tenantId).Include(x => x.Supplier).Include(x => x.Warehouse).Include(x => x.Items).ThenInclude(x => x.ProductVariant).AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> SupplierExistsAsync(Guid tenantId, Guid supplierId, CancellationToken cancellationToken = default) =>
        db.Suppliers.AnyAsync(x => x.TenantId == tenantId && x.Id == supplierId, cancellationToken);

    public Task<bool> WarehouseExistsAsync(Guid tenantId, Guid warehouseId, CancellationToken cancellationToken = default) =>
        db.Warehouses.AnyAsync(x => x.TenantId == tenantId && x.Id == warehouseId, cancellationToken);

    public Task<bool> VariantsBelongToTenantAsync(Guid tenantId, IEnumerable<Guid> variantIds, CancellationToken cancellationToken = default)
    {
        var ids = variantIds.Distinct().ToList();
        return db.ProductVariants.CountAsync(x => x.TenantId == tenantId && ids.Contains(x.Id), cancellationToken)
            .ContinueWith(t => t.Result == ids.Count, cancellationToken);
    }

    public Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
    {
        db.PurchaseOrders.Add(order);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

