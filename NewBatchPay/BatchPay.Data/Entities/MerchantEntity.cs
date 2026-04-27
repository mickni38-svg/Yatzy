namespace BatchPay.Data.Entities;

public sealed class MerchantEntity : DirectoryEntryEntity
{
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    // Kontakt (B2B)
    public string ContactEmail { get; set; } = "";
    public string? ContactPhone { get; set; }

    // Adresse (valgfrit)
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; } // "DK"

    // Navigation
    public MerchantIntegrationEntity? Integration { get; set; }
}
