namespace Discord;

/// <summary>
///     Represents the type of entitlement.
/// </summary>
public enum EntitlementType
{
    /// <summary>
    ///     The entitlement was purchased by user.
    /// </summary>
    Purchase = 1,

    /// <summary>
    ///     Entitlement for Discord Nitro subscription.
    /// </summary>
    PremiumSubscription = 2,

    /// <summary>
    ///     Entitlement was gifted by developer.
    /// </summary>
    DeveloperGift = 3,

    /// <summary>
    ///     Entitlement was purchased by a dev in application test mode.
    /// </summary>
    TestModePurchase = 4,

    /// <summary>
    ///     Entitlement was granted when the SKU was free.
    /// </summary>
    FreePurchase = 5,

    /// <summary>
    ///     Entitlement was gifted by another user.
    /// </summary>
    UserGift = 6,

    /// <summary>
    ///     Entitlement was claimed by user for free as a Nitro Subscriber.
    /// </summary>
    PremiumPurchase = 7,

    /// <summary>
    ///     The entitlement was purchased as an app subscription.
    /// </summary>
    ApplicationSubscription = 8
}
