using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TenantRepository(AppDbContext db) : ITenantRepository
{
    public Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.Tenants.AsNoTracking().ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Tenant>)t.Result, cancellationToken);

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        db.Tenants.AnyAsync(x => x.Code == code, cancellationToken);

    public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        db.Tenants.Add(tenant);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}

