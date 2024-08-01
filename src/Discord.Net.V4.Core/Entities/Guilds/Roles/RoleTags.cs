using Discord.Models;

namespace Discord;

/// <summary>
///     Provides tags related to a discord role.
/// </summary>
public readonly struct RoleTags :
    IConstructable<RoleTags, IRoleTagsModel>,
    IEquatable<RoleTags>
{
    /// <summary>
    ///     Gets the identifier of the bot that this role belongs to, if it does.
    /// </summary>
    /// <returns>
    ///     A <see langword="ulong" /> if this role belongs to a bot; otherwise
    ///     <see langword="null" />.
    /// </returns>
    public ulong? BotId { get; }

    /// <summary>
    ///     Gets the identifier of the integration that this role belongs to, if it does.
    /// </summary>
    /// <returns>
    ///     A <see langword="ulong" /> if this role belongs to an integration; otherwise
    ///     <see langword="null" />.
    /// </returns>
    public ulong? IntegrationId { get; }

    /// <summary>
    ///     Gets if this role is the guild's premium subscriber (booster) role.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this role is the guild's premium subscriber role;
    ///     otherwise <see langword="false" />.
    /// </returns>
    public bool IsPremiumSubscriberRole { get; }

    /// <summary>
    ///     Gets the subscription SKU ID if this role is available for purchase.
    /// </summary>
    public ulong? SubscriptionListingId { get; }

    /// <summary>
    ///     Gets whether this role can be bought.
    /// </summary>
    public bool AvailableForPurchase { get; }

    /// <summary>
    ///     Gets whether this role is a guilds linked role.
    /// </summary>
    public bool GuildConnections { get; }

    internal RoleTags(ulong? botId, ulong? integrationId, bool isPremiumSubscriber, ulong? subscriptionListingId,
        bool availableForPurchase, bool guildConnections)
    {
        BotId = botId;
        IntegrationId = integrationId;
        IsPremiumSubscriberRole = isPremiumSubscriber;
        SubscriptionListingId = subscriptionListingId;
        AvailableForPurchase = availableForPurchase;
        GuildConnections = guildConnections;
    }

    public static RoleTags Construct(IDiscordClient client, IRoleTagsModel model)
    {
        return new RoleTags(
            model.BotId,
            model.IntegrationId,
            model.IsPremiumSubscriberRole,
            model.SubscriptionListingId,
            model.AvailableForPurchase,
            model.IsGuildConnection
        );
    }

    public bool Equals(RoleTags other) => BotId == other.BotId && IntegrationId == other.IntegrationId && IsPremiumSubscriberRole == other.IsPremiumSubscriberRole && SubscriptionListingId == other.SubscriptionListingId && AvailableForPurchase == other.AvailableForPurchase && GuildConnections == other.GuildConnections;

    public override bool Equals(object? obj) => obj is RoleTags other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BotId, IntegrationId, IsPremiumSubscriberRole, SubscriptionListingId, AvailableForPurchase, GuildConnections);

    public static bool operator ==(RoleTags left, RoleTags right) => left.Equals(right);

    public static bool operator !=(RoleTags left, RoleTags right) => !left.Equals(right);
}
