using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface ISalesOrderService
{
    Task<IReadOnlyList<SalesOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SalesOrder> CreateAsync(CreateSalesRequest request, CancellationToken cancellationToken = default);
}

