namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.MentionableSelect"/> type.
/// </summary>
public class MentionableSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    internal MentionableSelectInputComponentInfo(Builders.MentionableSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
