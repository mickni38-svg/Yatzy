using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Data.Seed;

public sealed class DatabaseSeeder( BatchPayContext db )
{
    public async Task SeedMerchantsAsync( CancellationToken ct )
    {
        var merchants = new[]
        {
            new
            {
                DisplayName = "Sticks & Sushi",
                Handle = "sticksandsushi",
                Website = "https://sticksandsushi.com",
                ContactEmail = "kontakt@sticksandsushi.com",
                City = "København"
            },
            new
            {
                DisplayName = "Gasoline Grill",
                Handle = "gasolinegrill",
                Website = "https://gasolinegrill.com",
                ContactEmail = "kontakt@gasolinegrill.com",
                City = "København"
            }
        };

        // MODIFIED: Query the base DbSet and filter by type
        var existingMerchantHandles = await db.DirectoryEntries
            .OfType<MerchantEntity>()
            .Select( m => m.Handle )
            .ToHashSetAsync( StringComparer.OrdinalIgnoreCase, ct );

        foreach (var m in merchants)
        {
            if (existingMerchantHandles.Contains( m.Handle ))
                continue;

            var merchant = new MerchantEntity
            {
                DisplayName = m.DisplayName,
                Handle = m.Handle,
                WebsiteUrl = m.Website,
                ContactEmail = m.ContactEmail,
                City = m.City,
                CountryCode = "DK",
                IsActive = true
                // CreatedAtUtc is likely handled by the database or entity constructor now
            };

            // MODIFIED: Add to the base DbSet
            db.DirectoryEntries.Add( merchant );
            await db.SaveChangesAsync( ct ); // so we get merchant.Id

            var integration = new MerchantIntegrationEntity
            {
                MerchantId = merchant.Id,
                WebhookUrl = $"https://example.com/webhooks/sbys/{merchant.Handle}",
                DefaultReturnUrl = merchant.WebsiteUrl,
                AllowedOrigin = merchant.WebsiteUrl,
                ApiKeyHash = $"DEV_API_KEY_{merchant.Handle.ToUpperInvariant()}",
                SigningSecretHash = $"DEV_SECRET_{merchant.Handle.ToUpperInvariant()}",
                IsEnabled = true
                // UpdatedAtUtc is likely handled by the database or entity constructor now
            };

            db.MerchantIntegrations.Add( integration );
        }

        await db.SaveChangesAsync( ct );
    }

    public async Task SeedUsersAsync( CancellationToken ct )
    {
        var seed = new (string name, string handle)[]
        {
            ("Mads Grønlund", "mads"),
            ("Nikolaj Holm", "nikolaj"),
            ("Steff Warto", "steff"),
            ("Adda Algren", "adda"),
            ("Sofie Lund", "sofie"),
            ("Emil Jensen", "emil"),
            ("Sara Nørgaard", "sara"),
            ("Oliver Dahl", "oliver"),
            ("Freja Hansen", "freja"),
            ("Noah Pedersen", "noah"),
            ("Alma Kristensen", "alma"),
            ("Victor Møller", "victor"),
            ("Laura Nielsen", "laura"),
            ("Lucas Rasmussen", "lucas"),
            ("Anna Mortensen", "anna"),
            ("Oscar Sørensen", "oscar"),
            ("Clara Thomsen", "clara"),
            ("Malthe Eriksen", "malthe"),
            ("Ida Andersen", "ida"),
            ("William Skov", "william"),
        };

        // MODIFIED: Query the base DbSet and filter by type
        var existing = await db.DirectoryEntries
            .OfType<UserEntity>()
            .Select( u => u.Handle )
            .ToListAsync( ct );
        var set = new HashSet<string>( existing, StringComparer.OrdinalIgnoreCase );

        foreach (var (name, handle) in seed)
        {
            if (set.Contains( handle )) continue;

            // MODIFIED: Add to the base DbSet
            db.DirectoryEntries.Add( new UserEntity
            {
                DisplayName = name,
                Handle = handle,
                Email = $"{handle}@example.com" // Email is now a required field
            } );
        }

        await db.SaveChangesAsync( ct );
    }
}
