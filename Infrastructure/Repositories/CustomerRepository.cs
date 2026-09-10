using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.Customers.Where(x => x.TenantId == tenantId).AsNoTracking().ToListAsync(cancellationToken);

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        db.Customers.Add(customer);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

