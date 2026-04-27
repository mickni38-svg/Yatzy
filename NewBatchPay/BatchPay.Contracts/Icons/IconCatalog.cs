namespace BatchPay.Contracts.Icons;

/// <summary>
/// IMPORTANT:
/// En allow-list over ikoner som backend accepterer.
/// Frontend kan også bruge samme liste (hvis du refererer Contracts fra frontend).
/// Gem kun IconKey i DB.
/// </summary>
public static class IconCatalog
{
    // NOTE:
    // Disse keys skal matche filnavne i MAUI: fx "icon_pizza.svg" => key "pizza"
    public static readonly IReadOnlyList<string> Keys = new[]
    {
        "pizza",
        "beer",
        "trip",
        "party",
        "work",
        "coffee",
        "burger",
        "sushi",
        "taco",
        "music",
        "movie",
        "food",
        "cafe",
        "education",
        "fun",
        "gift",
        "health",
        "housing",
        "other",
        "shopping",
        "transport",
        "travel",
        "utilities"
    };

    public static bool IsValid( string? iconKey )
        => !string.IsNullOrWhiteSpace( iconKey ) && Keys.Contains( iconKey.Trim() );
}
