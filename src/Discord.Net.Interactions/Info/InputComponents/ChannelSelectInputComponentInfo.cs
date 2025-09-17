using Discord.Interactions.Builders.Modals.Inputs;

namespace Discord.Interactions.Info.InputComponents;

public class ChannelSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    public ChannelSelectInputComponentInfo(ChannelSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
