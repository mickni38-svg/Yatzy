namespace BatchPay.Data.Entities;

/// <summary>
/// Abstract base class for any entity that can be a friend, a group member, etc.
/// Represents a single entry in a consolidated directory of users and merchants.
/// </summary>
public abstract class DirectoryEntryEntity
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Handle { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}