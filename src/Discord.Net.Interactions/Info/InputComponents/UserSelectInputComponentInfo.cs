namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.UserSelect"/> type.
/// </summary>
public class UserSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    internal UserSelectInputComponentInfo(Builders.UserSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
