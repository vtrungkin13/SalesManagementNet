using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Infrastructure.Services;

public class InventoryService(IInventoryRepository repository, ITenantContext tenant) : IInventoryService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public Task<Inventory> AdjustAsync(StockAdjustmentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Quantity == 0)
        {
            throw new ArgumentException("Quantity cannot be zero.");
        }

        return repository.ExecuteInTransactionAsync(async () =>
        {
            var variant = await repository.GetVariantAsync(TenantId, request.ProductVariantId, cancellationToken)
                ?? throw new ArgumentException("Invalid product variant.");

            var inventory = await repository.GetAsync(TenantId, request.WarehouseId, request.ProductVariantId, cancellationToken);
            if (inventory is null)
            {
                inventory = new Inventory
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    WarehouseId = request.WarehouseId,
                    ProductVariantId = variant.Id,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    UpdatedAt = DateTime.UtcNow
                };
                await repository.AddAsync(inventory, cancellationToken);
            }

            var newQuantity = inventory.Quantity + request.Quantity;
            if (newQuantity < 0)
            {
                throw new InvalidOperationException("Insufficient stock.");
            }

            inventory.Quantity = newQuantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            await repository.AddTransactionAsync(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                WarehouseId = request.WarehouseId,
                ProductVariantId = request.ProductVariantId,
                Type = request.Quantity > 0 ? TransactionType.IN : TransactionType.OUT,
                Quantity = Math.Abs(request.Quantity),
                BalanceAfter = newQuantity,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await repository.SaveChangesAsync(cancellationToken);
            return inventory;
        }, cancellationToken);
    }
}

