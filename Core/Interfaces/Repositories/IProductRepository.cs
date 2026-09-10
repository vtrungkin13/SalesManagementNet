using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ProductCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task<bool> SkuExistsAsync(Guid tenantId, string sku, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(Guid tenantId, Guid categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

