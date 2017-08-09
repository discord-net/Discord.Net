using Discord.Serialization;

namespace Discord
{
    public enum PermissionTarget
    {
        [ModelEnum("role")]
        Role,
        [ModelEnum("user")]
        User
    }
}
