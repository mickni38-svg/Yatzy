using BatchPay.Contracts.Dto;
using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FriendsController : ControllerBase
{
    private readonly IFriendService _friendService;

    public FriendsController(IFriendService friendService)
    {
        _friendService = friendService;
    }

    [HttpGet("{requesterId:int}")]
    public async Task<IReadOnlyList<UserDto>> GetFriends(int requesterId, CancellationToken ct)
    {
        return await _friendService.GetFriendsAsync(requesterId, ct);
    }

    [HttpPost]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendRequestDto dto, CancellationToken ct)
    {
        // The DTO is now simpler: (int RequesterId, int ReceiverId)
        var success = await _friendService.AddFriendAsync(dto, ct);
        return success ? Ok() : BadRequest("Failed to add friend.");
    }

    // GET /api/friends/directory/{requesterId}
    [HttpGet( "directory/{requesterId:int}" )]
    public Task<IReadOnlyList<DirectoryEntryDto>> GetFriendsDirectory( int requesterId, CancellationToken ct )
        => _friendService.GetFriendsDirectoryAsync( requesterId, ct );
}
