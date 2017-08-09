using Discord.Serialization;

namespace Discord
{
    public enum UserStatus
    {
        Offline,
        [ModelEnumValue("online")]
        Online,
        [ModelEnumValue("idle")]
        Idle,
        [ModelEnumValue("idle", EnumValueType.WriteOnly)]
        AFK,
        [ModelEnumValue("dnd")]
        DoNotDisturb,
        Invisible,
    }
}
