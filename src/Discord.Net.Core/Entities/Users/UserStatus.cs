using Discord.Serialization;

namespace Discord
{
    public enum UserStatus
    {
        Offline,
        [ModelEnum("online")]
        Online,
        [ModelEnum("idle")]
        Idle,
        [ModelEnum("idle", EnumValueType.WriteOnly)]
        AFK,
        [ModelEnum("dnd")]
        DoNotDisturb,
        Invisible,
    }
}
