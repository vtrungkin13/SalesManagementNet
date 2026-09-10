using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface ISupplierService
{
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Supplier> CreateAsync(CreatePartnerRequest request, CancellationToken cancellationToken = default);
}

