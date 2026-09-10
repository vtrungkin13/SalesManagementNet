using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
}

