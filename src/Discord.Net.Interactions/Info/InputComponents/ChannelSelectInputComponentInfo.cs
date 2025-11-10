namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.ChannelSelect"/> type.
/// </summary>
public class ChannelSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    internal ChannelSelectInputComponentInfo(Builders.ChannelSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
