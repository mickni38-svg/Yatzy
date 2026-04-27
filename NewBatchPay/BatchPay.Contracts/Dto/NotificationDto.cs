namespace BatchPay.Contracts.Dto;

public sealed record NotificationDto(
    int Id,
    int ToUserId,
    string Type,
    string Title,
    string Body,
    string? LinkUrl,
    int? GroupPaymentId,
    bool IsRead,
    DateTime CreatedAtUtc );