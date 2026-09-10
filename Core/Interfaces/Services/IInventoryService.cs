using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Inventory> AdjustAsync(StockAdjustmentRequest request, CancellationToken cancellationToken = default);
}

