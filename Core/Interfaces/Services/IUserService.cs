using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface IUserService
{
    Task<object> GetMeAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AppUser> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}

