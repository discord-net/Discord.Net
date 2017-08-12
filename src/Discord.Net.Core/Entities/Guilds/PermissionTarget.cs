using Discord.Serialization;

namespace Discord
{
    [ModelStringEnum]
    public enum PermissionTarget
    {
        [ModelEnumValue("role")]
        Role,
        [ModelEnumValue("member")]
        User
    }
}
