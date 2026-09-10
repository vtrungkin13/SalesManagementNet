using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class TenantService(ITenantRepository repository) : ITenantService
{
    public Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public async Task<Tenant> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsByCodeAsync(request.Code, cancellationToken))
        {
            throw new InvalidOperationException("Tenant code already exists.");
        }

        var now = DateTime.UtcNow;
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            CreatedAt = now,
            UpdatedAt = now
        };

        await repository.AddAsync(tenant, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return tenant;
    }
}

