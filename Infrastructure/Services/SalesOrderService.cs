using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Infrastructure.Services;

public class SalesOrderService(ISalesOrderRepository repository, ITenantContext tenant) : ISalesOrderService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<SalesOrder>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public Task<SalesOrder> CreateAsync(CreateSalesRequest request, CancellationToken cancellationToken = default) =>
        repository.ExecuteInTransactionAsync(async () =>
        {
            if (!await repository.CustomerExistsAsync(TenantId, request.CustomerId, cancellationToken) ||
                !await repository.WarehouseExistsAsync(TenantId, request.WarehouseId, cancellationToken))
            {
                throw new ArgumentException("Invalid customer or warehouse.");
            }

            if (request.Items.Count == 0)
            {
                throw new ArgumentException("Sales order must contain at least one item.");
            }

            var order = new SalesOrder
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                CustomerId = request.CustomerId,
                WarehouseId = request.WarehouseId,
                OrderNumber = $"SO-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                Status = "COMPLETED",
                OrderDate = DateTime.UtcNow,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var itemRequest in request.Items)
            {
                var inventory = await repository.GetInventoryAsync(
                    TenantId, request.WarehouseId, itemRequest.ProductVariantId, cancellationToken)
                    ?? throw new InvalidOperationException("Inventory not found.");

                if (inventory.AvailableQuantity < itemRequest.Quantity)
                {
                    throw new InvalidOperationException("Insufficient stock.");
                }

                inventory.Quantity -= itemRequest.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;

                var total = itemRequest.Quantity * itemRequest.UnitPrice - itemRequest.Discount + itemRequest.Tax;
                order.Items.Add(new SalesOrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductVariantId = itemRequest.ProductVariantId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    Discount = itemRequest.Discount,
                    Tax = itemRequest.Tax,
                    Total = total
                });

                await repository.AddInventoryTransactionAsync(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    WarehouseId = request.WarehouseId,
                    ProductVariantId = itemRequest.ProductVariantId,
                    Type = TransactionType.OUT,
                    Quantity = itemRequest.Quantity,
                    BalanceAfter = inventory.Quantity,
                    ReferenceType = "SalesOrder",
                    ReferenceId = order.Id,
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            order.Subtotal = order.Items.Sum(x => x.Quantity * x.UnitPrice);
            order.Discount = order.Items.Sum(x => x.Discount);
            order.Tax = order.Items.Sum(x => x.Tax);
            order.Total = order.Subtotal - order.Discount + order.Tax;

            await repository.AddAsync(order, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return order;
        }, cancellationToken);
}

