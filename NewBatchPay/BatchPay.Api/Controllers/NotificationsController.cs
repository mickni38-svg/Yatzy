using BatchPay.Contracts.Dto;
using BatchPay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route( "api/notifications" )]
public sealed class NotificationsController : ControllerBase
{
    private readonly BatchPayContext _db;

    public NotificationsController( BatchPayContext db )
    {
        _db = db;
    }

    [HttpGet( "for-user/{userId:int}" )]
    public async Task<IReadOnlyList<NotificationDto>> GetForUser( int userId, CancellationToken ct )
    {
        return await _db.Notifications
            .Where( n => n.ToUserId == userId )
            .OrderByDescending( n => n.CreatedAtUtc )
            .Select( n => new NotificationDto(
                n.Id,
                n.ToUserId,
                n.Type,
                n.Title,
                n.Body,
                n.LinkUrl,
                n.GroupPaymentId,
                n.IsRead,
                n.CreatedAtUtc ) )
            .ToListAsync( ct );
    }
}