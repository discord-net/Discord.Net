namespace Discord;

/// <summary>
///     Represents a guild product purchase.
/// </summary>
public readonly struct GuildProductPurchase
{
    /// <summary>
    ///     Gets the ID of the listing.
    /// </summary>
    public readonly ulong ListingId;

    /// <summary>
    ///    Gets the name of the product.
    /// </summary>
    public readonly string ProductName;

    internal GuildProductPurchase(ulong listingId, string productName)
    {
        ListingId = listingId;
        ProductName = productName;
    }
}
