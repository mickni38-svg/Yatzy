using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DirectoryController : ControllerBase
{
    private readonly IDirectoryService _directoryService;

    public DirectoryController(IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDirectory(CancellationToken ct)
    {
        var entries = await _directoryService.GetDirectoryAsync(ct);
        return Ok(entries);
    }
}
