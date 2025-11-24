namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.ChannelSelect"/> type.
/// </summary>
public class ChannelSelectComponentInfo : SnowflakeSelectComponentInfo
{
    internal ChannelSelectComponentInfo(Builders.ChannelSelectComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
