using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
}

