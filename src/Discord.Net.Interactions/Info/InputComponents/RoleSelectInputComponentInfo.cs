namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.RoleSelect"/> type.
/// </summary>
public class RoleSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    internal RoleSelectInputComponentInfo(Builders.RoleSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
