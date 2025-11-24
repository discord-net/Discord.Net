namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.RoleSelect"/> type.
/// </summary>
public class RoleSelectComponentInfo : SnowflakeSelectComponentInfo
{
    internal RoleSelectComponentInfo(Builders.RoleSelectComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
