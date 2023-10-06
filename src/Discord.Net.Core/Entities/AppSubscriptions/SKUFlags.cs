using System;

namespace Discord;

[Flags]
public enum SKUFlags
{
    GuildSubscription = 1 << 7,

    UserSubscription = 1 << 8
}
