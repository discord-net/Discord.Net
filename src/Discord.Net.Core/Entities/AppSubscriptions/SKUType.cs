namespace Discord;

public enum SKUType
{
    /// <summary>
    ///     Durable one-time purchase.
    /// </summary>
    Durable = 2,

    /// <summary>
    ///     Consumable one-time purchase.
    /// </summary>
    Consumable = 3,

    /// <summary>
    ///     Represents a recurring subscription.
    /// </summary>
    Subscription = 5,

    /// <summary>
    ///     System-generated group for each <see cref="Subscription"/> SKU created.
    /// </summary>
    SubscriptionGroup = 6,
}
