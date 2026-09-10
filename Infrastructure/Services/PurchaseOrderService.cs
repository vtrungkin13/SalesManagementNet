using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Infrastructure.Services;

public class PurchaseOrderService(IPurchaseOrderRepository repository, ITenantContext tenant) : IPurchaseOrderService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<PurchaseOrder> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        if (!await repository.SupplierExistsAsync(TenantId, request.SupplierId, cancellationToken) ||
            !await repository.WarehouseExistsAsync(TenantId, request.WarehouseId, cancellationToken))
        {
            throw new ArgumentException("Invalid supplier or warehouse.");
        }

        if (request.Items.Count == 0 || !await repository.VariantsBelongToTenantAsync(
                TenantId, request.Items.Select(x => x.ProductVariantId), cancellationToken))
        {
            throw new ArgumentException("Invalid purchase order items.");
        }

        var items = request.Items.Select(x => new PurchaseOrderItem
        {
            Id = Guid.NewGuid(),
            ProductVariantId = x.ProductVariantId,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            Discount = x.Discount,
            Tax = x.Tax,
            Total = x.Quantity * x.UnitPrice - x.Discount + x.Tax
        }).ToList();

        var subtotal = items.Sum(x => x.Quantity * x.UnitPrice);
        var discount = items.Sum(x => x.Discount);
        var tax = items.Sum(x => x.Tax);
        var order = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            SupplierId = request.SupplierId,
            WarehouseId = request.WarehouseId,
            OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Status = PurchaseStatus.PENDING,
            OrderDate = DateTime.UtcNow,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            Total = subtotal - discount + tax,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = items
        };

        await repository.AddAsync(order, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return order;
    }
}

