using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class WarehouseService(IWarehouseRepository repository, ITenantContext tenant) : IWarehouseService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<Warehouse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Code = request.Code,
            Name = request.Name,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(warehouse, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return warehouse;
    }
}

