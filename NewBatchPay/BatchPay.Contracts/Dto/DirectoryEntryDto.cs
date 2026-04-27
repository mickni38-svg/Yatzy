namespace BatchPay.Contracts.Dto;

/// <summary>
/// Angiver hvilken type entry der vises i "Find"-listen.
/// </summary>
public enum DirectoryEntryType
{
    User = 0,
    Merchant = 1
}

/// <summary>
/// Fælles DTO til at vise både Users og Merchants i samme liste i appen.
/// </summary>
public sealed record DirectoryEntryDto
(
    DirectoryEntryType Type,
    int Id,
    string DisplayName,
    string Handle,
    string? Subtitle = null,   // fx by, kategori, “Spisested”, etc.
    string? LogoUrl = null     // hvis merchant har logo
);
