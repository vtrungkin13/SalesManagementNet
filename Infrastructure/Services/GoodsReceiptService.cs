using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Infrastructure.Services;

public class GoodsReceiptService(IGoodsReceiptRepository repository, ITenantContext tenant) : IGoodsReceiptService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<GoodsReceipt> CreateAsync(CreateReceiptRequest request, CancellationToken cancellationToken = default) =>
        repository.ExecuteInTransactionAsync(async () =>
        {
            if (!await repository.PurchaseOrderExistsAsync(TenantId, request.PurchaseOrderId, cancellationToken) ||
                !await repository.WarehouseExistsAsync(TenantId, request.WarehouseId, cancellationToken))
            {
                throw new ArgumentException("Invalid purchase order or warehouse.");
            }

            if (request.Items.Count == 0 || !await repository.VariantsBelongToTenantAsync(
                    TenantId, request.Items.Select(x => x.ProductVariantId), cancellationToken))
            {
                throw new ArgumentException("Invalid receipt items.");
            }

            var purchaseOrder = await repository.GetPurchaseOrderAsync(
                request.PurchaseOrderId, TenantId, cancellationToken);

            if (purchaseOrder is null)
            {
                throw new KeyNotFoundException("Purchase order not found.");
            }

            var receipt = new GoodsReceipt
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                PurchaseOrderId = request.PurchaseOrderId,
                WarehouseId = request.WarehouseId,
                ReceiptNumber = $"GR-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                ReceiptDate = DateTime.UtcNow,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                Items = request.Items.Select(x => new GoodsReceiptItem
                {
                    Id = Guid.NewGuid(),
                    ProductVariantId = x.ProductVariantId,
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost
                }).ToList()
            };

            foreach (var item in receipt.Items)
            {
                var inventory = await repository.GetInventoryAsync(
                    TenantId, request.WarehouseId, item.ProductVariantId, cancellationToken);

                if (inventory is null)
                {
                    inventory = new Inventory
                    {
                        Id = Guid.NewGuid(),
                        TenantId = TenantId,
                        WarehouseId = request.WarehouseId,
                        ProductVariantId = item.ProductVariantId,
                        Quantity = 0,
                        ReservedQuantity = 0,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await repository.AddInventoryAsync(inventory, cancellationToken);
                }

                inventory.Quantity += item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;
                await repository.AddInventoryTransactionAsync(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    WarehouseId = request.WarehouseId,
                    ProductVariantId = item.ProductVariantId,
                    Type = TransactionType.IN,
                    Quantity = item.Quantity,
                    BalanceAfter = inventory.Quantity,
                    ReferenceType = "GoodsReceipt",
                    ReferenceId = receipt.Id,
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            purchaseOrder.Status = PurchaseStatus.RECEIVED;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;
            await repository.AddAsync(receipt, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return receipt;
        }, cancellationToken);

}
