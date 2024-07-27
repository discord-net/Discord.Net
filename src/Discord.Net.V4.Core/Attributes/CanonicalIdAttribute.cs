namespace Discord.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class CanonicalIdAttribute(CanonicalIdType type) : Attribute
{
    public CanonicalIdType Type { get; } = type;
}

public enum CanonicalIdType
{
    Stickers,
    Guild,
    Role,
    GuildSticker,
    GuildEmote,
    GuildScheduledEvent,
    GuildMember,
    Channel,
    User,
    Invite,
    Webhook
}
