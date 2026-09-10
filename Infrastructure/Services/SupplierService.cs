using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class SupplierService(ISupplierRepository repository, ITenantContext tenant) : ISupplierService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<Supplier> CreateAsync(CreatePartnerRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Code = request.Code,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(supplier, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return supplier;
    }
}

