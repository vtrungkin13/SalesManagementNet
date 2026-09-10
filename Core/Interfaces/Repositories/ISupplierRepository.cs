using Core.Entities;

namespace Core.Interfaces;

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

