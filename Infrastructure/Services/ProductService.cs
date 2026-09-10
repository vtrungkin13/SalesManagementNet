using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class ProductService(IProductRepository repository, ITenantContext tenant) : IProductService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (!await repository.CategoryExistsAsync(TenantId, request.CategoryId, cancellationToken))
        {
            throw new ArgumentException("Invalid category.");
        }

        if (await repository.ProductCodeExistsAsync(TenantId, request.Code, cancellationToken))
        {
            throw new InvalidOperationException("Product code already exists.");
        }

        var sku = string.IsNullOrWhiteSpace(request.Sku) ? $"{request.Code}-DEFAULT" : request.Sku!;
        if (await repository.SkuExistsAsync(TenantId, sku, cancellationToken))
        {
            throw new InvalidOperationException("SKU already exists.");
        }

        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            CategoryId = request.CategoryId,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            ProductId = product.Id,
            Sku = sku,
            Name = request.Name,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            IsDefault = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await repository.AddAsync(product, cancellationToken);
        await repository.AddVariantAsync(variant, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return product;
    }
}

