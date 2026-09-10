using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface ITenantService
{
    Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tenant> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
}

