using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class CustomerService(ICustomerRepository repository, ITenantContext tenant) : ICustomerService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<Customer> CreateAsync(CreatePartnerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
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

        await repository.AddAsync(customer, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return customer;
    }
}

