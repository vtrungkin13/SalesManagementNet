using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class UserService(IUserRepository repository, ITenantContext tenant) : IUserService
{
    private Guid TenantId => tenant.TenantId ?? throw new UnauthorizedAccessException("Tenant context is required.");

    public async Task<object> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var userId = tenant.UserId ?? throw new UnauthorizedAccessException();
        var user = await repository.GetByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        return new
        {
            id = user.Id,
            user.Email,
            user.FullName,
            user.TenantId,
            Roles = user.UserRoles.Select(x => x.Role.Name)
        };
    }

    public Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetByTenantAsync(TenantId, cancellationToken);

    public async Task<AppUser> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var role = await repository.GetRoleByNameAsync(request.Role, cancellationToken)
            ?? throw new KeyNotFoundException("Role not found.");

        var now = DateTime.UtcNow;
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Phone = request.Phone,
            CreatedAt = now,
            UpdatedAt = now
        };

        await repository.AddAsync(user, cancellationToken);
        await repository.AddRoleAsync(new UserRole { UserId = user.Id, RoleId = role.Id }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return user;
    }
}

