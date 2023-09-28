namespace Discord;

public enum SKUType
{
    /// <summary>
    ///     Represents a recurring subscription.
    /// </summary>
    Subscription = 5,

    /// <summary>
    ///     System-generated group for each <see cref="SKUType.Subscription"/> SKU created.
    /// </summary>
    SubscriptionGroup = 6,
}
