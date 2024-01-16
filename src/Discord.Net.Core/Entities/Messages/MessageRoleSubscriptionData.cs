namespace Discord;

/// <summary>
///     Represents a role subscription data in <see cref="IMessage"/>.
/// </summary>
public class MessageRoleSubscriptionData
{
    /// <summary>
    ///     Gets the id of the sku and listing that the user is subscribed to.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    ///     Gets the name of the tier that the user is subscribed to.
    /// </summary>
    public string TierName { get; }

    /// <summary>
    ///     Gets the cumulative number of months that the user has been subscribed for.
    /// </summary>
    public int MonthsSubscribed { get; }

    /// <summary>
    ///     Gets whether this notification is for a renewal rather than a new purchase.
    /// </summary>
    public bool IsRenewal { get; }

    internal MessageRoleSubscriptionData(ulong id, string tierName, int monthsSubscribed, bool isRenewal)
    {
        Id = id;
        TierName = tierName;
        MonthsSubscribed = monthsSubscribed;
        IsRenewal = isRenewal;
    }
}
