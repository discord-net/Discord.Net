namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.MentionableSelect"/> type.
/// </summary>
public class MentionableSelectComponentInfo : SnowflakeSelectComponentInfo
{
    internal MentionableSelectComponentInfo(Builders.MentionableSelectComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
