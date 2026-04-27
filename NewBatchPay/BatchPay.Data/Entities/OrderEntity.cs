using System;
using System.Collections.Generic;

namespace BatchPay.Data.Entities;

public sealed class OrderEntity
{
    public int Id { get; set; }

    public int GroupPaymentMemberId { get; set; }
    public GroupPaymentMemberEntity GroupPaymentMember { get; set; } = null!;

    // Valgfrit men praktisk til historik/debug/simulering
    public int? MerchantId { get; set; }  // peger typisk på MerchantEntity (DirectoryEntry)
    public string? MerchantName { get; set; }
    public string? MerchantLink { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<OrderLineEntity> Lines { get; set; } = new();
}