using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IWarehouseService
{
    Task<IReadOnlyList<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Warehouse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default);
}

