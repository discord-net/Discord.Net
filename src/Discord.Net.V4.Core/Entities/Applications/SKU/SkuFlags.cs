namespace Discord;

[Flags]
public enum SkuFlags : int
{
    Available = 1 << 2,
    GuildSubscription = 1 << 7,
    UserSubscription = 1 << 8,
}