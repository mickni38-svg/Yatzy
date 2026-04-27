using BatchPay.Contracts.Dto;

namespace BatchPay.Logic.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetForUserAsync( int userId, CancellationToken ct );
    Task MarkReadAsync( int notificationId, CancellationToken ct );
}