using Core.Entities;

namespace Core.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

