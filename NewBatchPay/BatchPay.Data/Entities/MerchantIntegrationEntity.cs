namespace BatchPay.Data.Entities;

public sealed class MerchantIntegrationEntity
{
    // 1:1 med Merchant (samme PK som FK)
    public int MerchantId { get; set; }

    // SBYS -> Merchant events (ALL_AUTHORIZED/EXPIRED/etc.)
    public string WebhookUrl { get; set; } = "";

    // Valgfrit: hvor SBYS kan sende brugeren hen bagefter
    public string? DefaultReturnUrl { get; set; }

    // Valgfrit: anti-misuse / domæne validering
    public string? AllowedOrigin { get; set; }

    // Adgangskontrol
    public bool IsEnabled { get; set; } = true;

    // Sikkerhed (gem hash/krypteret – ikke plain i prod)
    public string ApiKeyHash { get; set; } = "";
    public string SigningSecretHash { get; set; } = "";

    public DateTime UpdatedAtUtc { get; set; }

    // Navigation
    public MerchantEntity? Merchant { get; set; }
}
