using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a role subscription data in <see cref="IMessage" />.
/// </summary>
public readonly struct
    MessageRoleSubscriptionData : IModelConstructable<MessageRoleSubscriptionData, IMessageRoleSubscriptionData>
{
    /// <summary>
    ///     The id of the sku and listing that the user is subscribed to.
    /// </summary>
    public readonly ulong Id;

    /// <summary>
    ///     The name of the tier that the user is subscribed to.
    /// </summary>
    public readonly string TierName;

    /// <summary>
    ///     The cumulative number of months that the user has been subscribed for.
    /// </summary>
    public readonly int MonthsSubscribed;

    /// <summary>
    ///     Whether this notification is for a renewal rather than a new purchase.
    /// </summary>
    public readonly bool IsRenewal;

    internal MessageRoleSubscriptionData(ulong id, string tierName, int monthsSubscribed, bool isRenewal)
    {
        Id = id;
        TierName = tierName;
        MonthsSubscribed = monthsSubscribed;
        IsRenewal = isRenewal;
    }

    public static MessageRoleSubscriptionData Construct(IDiscordClient client, IMessageRoleSubscriptionData model)
        => new(model.RoleSubscriptionListingId, model.TierName, model.TotalMonthsSubscribed, model.IsRenewal);
}
