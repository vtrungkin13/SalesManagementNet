using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IGoodsReceiptService
{
    Task<GoodsReceipt> CreateAsync(CreateReceiptRequest request, CancellationToken cancellationToken = default);
}

