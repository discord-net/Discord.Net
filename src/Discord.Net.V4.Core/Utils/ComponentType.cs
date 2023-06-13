namespace Discord.Utils;

public static class ComponentTypeUtils
{
    public static bool IsSelectType(this ComponentType type) => type is ComponentType.ChannelSelect
        or ComponentType.SelectMenu or ComponentType.RoleSelect or ComponentType.UserSelect
        or ComponentType.MentionableSelect;
}
