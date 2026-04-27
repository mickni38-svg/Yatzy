using BatchPay.Contracts.Dto;
using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route("api/group-payments")]
public class GroupPaymentsController : ControllerBase
{
    private readonly IGroupPaymentService _groupPaymentService;

    public GroupPaymentsController(IGroupPaymentService groupPaymentService)
    {
        _groupPaymentService = groupPaymentService;
    }

    [HttpPost]
    public async Task<ActionResult<GroupPaymentDto>> Create(CreateGroupPaymentRequestDto dto, CancellationToken ct)
    {
        // The DTO now contains a List<int> MemberIds from the unified directory
        var result = await _groupPaymentService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("for-user/{userId:int}")]
    public async Task<IReadOnlyList<GroupPaymentDto>> GetForUser(int userId, CancellationToken ct)
    {
        return await _groupPaymentService.GetForUserAsync(userId, ct);
    }

    // Dummy endpoint for CreatedAtAction
    [HttpGet("{id:int}")]
    public Task<GroupPaymentDto> GetById(int id) => throw new NotImplementedException();
}