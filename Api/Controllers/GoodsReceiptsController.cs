using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/receipts")]
public class GoodsReceiptsController(IGoodsReceiptService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateReceiptRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await service.CreateAsync(request, cancellationToken));
    }
}
