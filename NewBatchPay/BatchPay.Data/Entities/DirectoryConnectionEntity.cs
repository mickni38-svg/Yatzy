namespace BatchPay.Data.Entities;

/// <summary>
/// En "connection" fra en bruger til enten en anden bruger eller et merchant.
/// Bruges til: venner + "følg spisested" uden at have 2 separate tabeller.
/// </summary>
public sealed class DirectoryConnectionEntity
{
    public int Id { get; set; }

    // Den bruger der "følger"/er ven med target
    public int OwnerUserId { get; set; }

    // Target kan være User eller Merchant
    public byte TargetType { get; set; } // 0=User, 1=Merchant
    public int TargetId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    // Soft disable (valgfri)
    public bool IsActive { get; set; } = true;

    // Navigation (kun for Owner; Target er polymorf og løses i queries)
    public UserEntity? OwnerUser { get; set; }
}
