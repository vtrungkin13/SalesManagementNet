using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class CategoryService(ICategoryRepository repository, ITenantContext tenant) : ICategoryService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<Category> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsByCodeAsync(TenantId, request.Code, cancellationToken))
        {
            throw new InvalidOperationException("Category code already exists.");
        }

        var now = DateTime.UtcNow;
        var category = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now
        };

        await repository.AddAsync(category, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return category;
    }
}

