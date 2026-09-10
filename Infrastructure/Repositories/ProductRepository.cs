using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await db.Products.Where(x => x.TenantId == tenantId).Include(x => x.Category).Include(x => x.Variants).AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> ProductCodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(x => x.TenantId == tenantId && x.Code == code, cancellationToken);

    public Task<bool> SkuExistsAsync(Guid tenantId, string sku, CancellationToken cancellationToken = default) =>
        db.ProductVariants.AnyAsync(x => x.TenantId == tenantId && x.Sku == sku, cancellationToken);

    public Task<bool> CategoryExistsAsync(Guid tenantId, Guid categoryId, CancellationToken cancellationToken = default) =>
        db.Categories.AnyAsync(x => x.Id == categoryId && x.TenantId == tenantId, cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        db.Products.Add(product);
        return Task.CompletedTask;
    }

    public Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default)
    {
        db.ProductVariants.Add(variant);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}

