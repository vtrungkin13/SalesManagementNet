using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/inventory")]
public class InventoryController(IInventoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust(
        StockAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await service.AdjustAsync(request, cancellationToken));
    }
}
