namespace BatchPay.Data.Entities;

public sealed class NotificationEntity
{
    public int Id { get; set; }

    public int ToUserId { get; set; }

    public string Type { get; set; } = "";   // fx "GroupPaymentCreated"
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";

    public string? LinkUrl { get; set; }     // merchant order link
    public int? GroupPaymentId { get; set; } // relation til group payment

    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}