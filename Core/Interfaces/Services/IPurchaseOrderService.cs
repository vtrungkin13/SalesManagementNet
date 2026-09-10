using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IPurchaseOrderService
{
    Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PurchaseOrder> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
}

