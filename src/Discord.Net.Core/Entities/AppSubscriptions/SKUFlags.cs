using System;

namespace Discord;

/// <summary>
///     SKU flags for subscriptions.
/// </summary>
[Flags]
public enum SKUFlags
{
    /// <summary>
    ///     The SKU is a guild subscription.
    /// </summary>
    GuildSubscription = 1 << 7,

    /// <summary>
    ///     The SKU is a user subscription.
    /// </summary>
    UserSubscription = 1 << 8
}
