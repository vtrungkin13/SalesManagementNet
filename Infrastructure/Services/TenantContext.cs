using System.Security.Claims;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
namespace Infrastructure.Services;

public sealed class TenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    public Guid? TenantId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue("tenant_id"), out var id) ? id : null;
    public Guid? UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public bool IsGlobalAdmin => accessor.HttpContext?.User.IsInRole("ADMIN") == true && TenantId is null;
}


