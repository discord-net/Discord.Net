namespace Discord;

public enum SubscriptionStatus
{
    /// <summary>
    ///     Subscription is active and scheduled to renew.
    /// </summary>
    Active = 0,

    /// <summary>
    ///     Subscription is active but will not renew.
    /// </summary>
    Ending = 1,

    /// <summary>
    ///      Subscription is inactive and not being charged.
    /// </summary>
    Inactive = 2
}
