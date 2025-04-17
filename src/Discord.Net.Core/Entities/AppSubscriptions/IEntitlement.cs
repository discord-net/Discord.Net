using System;

namespace Discord;

public interface IEntitlement : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the ID of the SKU this entitlement is for.
    /// </summary>
    ulong SkuId { get; }

    /// <summary>
    ///     Gets the ID of the user that is granted access to the entitlement's SKU.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the entitlement is for a guild.
    /// </remarks>
    ulong? UserId { get; }

    /// <summary>
    ///     Gets the ID of the guild that is granted access to the entitlement's SKU.
    /// </summary> 
    /// <remarks>
    ///     <see langword="null"/> if the entitlement is for a user.
    /// </remarks>
    ulong? GuildId { get; }

    /// <summary>
    ///     Gets the ID of the parent application.
    /// </summary>
    ulong ApplicationId { get; }

    /// <summary>
    ///     Gets the type of the entitlement.
    /// </summary>
    EntitlementType Type { get; }

    /// <summary>
    ///     Gets whether this entitlement has been consumed.
    /// </summary>
    /// <remarks>
    ///     Not applicable for App Subscriptions.
    /// </remarks>
    bool IsConsumed { get; }

    /// <summary>
    ///     Gets the start date at which the entitlement is valid.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> when using test entitlements.
    /// </remarks>
    DateTimeOffset? StartsAt { get; }

    /// <summary>
    ///     Gets the end date at which the entitlement is no longer valid.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> when using test entitlements.
    /// </remarks>
    DateTimeOffset? EndsAt { get; }
}
