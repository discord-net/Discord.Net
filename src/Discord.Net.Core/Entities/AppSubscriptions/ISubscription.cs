using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a subscription object.
/// </summary>
public interface ISubscription : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the ID of the user who is subscribed.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    ///     Gets the SKUs subscribed to.
    /// </summary>
    IReadOnlyCollection<ulong> SKUIds { get; }

    /// <summary>
    ///     Gets the entitlements granted for this subscription.
    /// </summary>
    IReadOnlyCollection<ulong> EntitlementIds { get; }

    /// <summary>
    ///     Gets the SKUs that this user will be subscribed to at renewal.
    /// </summary>
    IReadOnlyCollection<ulong> RenewalSKUIds { get; }

    /// <summary>
    ///     Gets the start of the current subscription period.
    /// </summary>
    DateTimeOffset CurrentPeriodStart { get; }

    /// <summary>
    ///     Gets end of the current subscription period.
    /// </summary>
    DateTimeOffset CurrentPeriodEnd { get; }

    /// <summary>
    ///     Gets the current status of the subscription.
    /// </summary>
    SubscriptionStatus Status { get; }

    /// <summary>
    ///     Gets when the subscription was canceled.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the subscription has not been canceled.
    /// </remarks>
    DateTimeOffset? CanceledAt { get; }

    /// <summary>
    ///     Gets country code of the payment source used to purchase the subscription.
    /// </summary>
    /// <remarks>
    ///     Requires an oauth scope.
    /// </remarks>
    string Country { get; }
}
