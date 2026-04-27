using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class NotificationService( BatchPayContext db ) : INotificationService
{
    public async Task<IReadOnlyList<NotificationDto>> GetForUserAsync( int userId, CancellationToken ct )
    {
        return await db.Notifications
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

    public async Task MarkReadAsync( int notificationId, CancellationToken ct )
    {
        var n = await db.Notifications.FirstOrDefaultAsync( x => x.Id == notificationId, ct );
        if (n is null) return;

        n.IsRead = true;
        await db.SaveChangesAsync( ct );
    }
}