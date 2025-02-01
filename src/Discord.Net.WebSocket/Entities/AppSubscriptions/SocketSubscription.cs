using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.WebSocket;

/// <summary>
///     Represents an application subscription.
/// </summary>
public class SocketSubscription : SocketEntity<ulong>, ISubscription
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> SKUIds { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> EntitlementIds { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> RenewalSKUIds { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CurrentPeriodStart { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CurrentPeriodEnd { get; private set; }

    /// <inheritdoc />
    public SubscriptionStatus Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CanceledAt { get; private set; }

    /// <inheritdoc />
    public string Country { get; private set; }

    internal SocketSubscription(DiscordSocketClient discord, ulong id) : base(discord, id)
    {
    }

    internal static SocketSubscription Create(DiscordSocketClient discord, API.Subscription model)
    {
        var s = new SocketSubscription(discord, model.Id);
        s.Update(model);
        return s;
    }

    internal void Update(API.Subscription model)
    {
        UserId = model.UserId;
        SKUIds = model.SKUIds.ToImmutableArray();
        EntitlementIds = model.EntitlementIds.ToImmutableArray();
        RenewalSKUIds = model.RenewalSKUIds?.ToImmutableArray() ?? [];
        CurrentPeriodStart = model.CurrentPeriodStart;
        CurrentPeriodEnd = model.CurrentPeriodEnd;
        Status = model.Status;
        CanceledAt = model.CanceledAt;
        Country = model.Country;
    }

    internal SocketSubscription Clone()
        => MemberwiseClone() as SocketSubscription;
}
