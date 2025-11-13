namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.UserSelect"/> type.
/// </summary>
public class UserSelectComponentInfo : SnowflakeSelectComponentInfo
{
    internal UserSelectComponentInfo(Builders.UserSelectComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
