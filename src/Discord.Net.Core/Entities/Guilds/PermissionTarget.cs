using Discord.Serialization;

namespace Discord
{
    public enum PermissionTarget
    {
        [ModelEnumValue("role")]
        Role,
        [ModelEnumValue("user")]
        User
    }
}
