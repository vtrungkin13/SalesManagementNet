namespace Core.Interfaces;

public interface ITenantContext 
{ 
    Guid? TenantId { get; } 
    bool IsGlobalAdmin { get; } 
    Guid? UserId { get; } 
}



