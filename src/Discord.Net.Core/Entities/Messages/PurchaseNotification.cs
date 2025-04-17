namespace Discord;

/// <summary>
///     Represents a purchase notification.
/// </summary>
public readonly struct PurchaseNotification
{
    /// <summary>
    ///     Gets the type of the purchase.
    /// </summary>
    public readonly PurchaseType Type;

    /// <summary>
    ///     Gets the purchased product.
    /// </summary>
    public readonly GuildProductPurchase? ProductPurchase;

    internal PurchaseNotification(PurchaseType type, GuildProductPurchase? productPurchase)
    {
        Type = type;
        ProductPurchase = productPurchase;
    }
}
