using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/tenants")]
public class TenantsController(ITenantService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = tenant.Id }, tenant);
    }
}
