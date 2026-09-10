namespace Core.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public AppUser User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}


