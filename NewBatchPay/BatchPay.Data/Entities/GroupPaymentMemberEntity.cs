using System.Collections.Generic;

namespace BatchPay.Data.Entities;

public sealed class GroupPaymentMemberEntity
{
    public int Id { get; set; }

    public int GroupPaymentId { get; set; }
    public int MemberId { get; set; } // DirectoryEntryEntity id (User/Merchant)
    public bool IsActive { get; set; } = false;

    public GroupPaymentEntity GroupPayment { get; set; } = null!;
    public DirectoryEntryEntity Member { get; set; } = null!;

    // ✅ NEW: 1 medlem kan have flere ordrer (fx ændringer/ny bestilling)
    public List<OrderEntity> Orders { get; set; } = new();
}