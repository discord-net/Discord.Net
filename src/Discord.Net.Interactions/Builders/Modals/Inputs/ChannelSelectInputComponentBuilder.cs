using Discord.Interactions.Info.InputComponents;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders.Modals.Inputs;

public class ChannelSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<ChannelSelectInputComponentInfo, ChannelSelectInputComponentBuilder>
{
    protected override ChannelSelectInputComponentBuilder Instance => this;

    public ChannelSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.ChannelSelect) { }

    public ChannelSelectInputComponentBuilder AddDefaulValue(IChannel channel)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channel.Id, SelectDefaultValueType.Channel));
        return this;
    }

    public ChannelSelectInputComponentBuilder AddDefaulValue(ulong channelId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channelId, SelectDefaultValueType.Channel));
        return this;
    }

    public ChannelSelectInputComponentBuilder AddDefaultValues(params IChannel[] channels)
    {
        _defaultValues.AddRange(channels.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Channel)));
        return this;
    }

    public ChannelSelectInputComponentBuilder AddDefaultValues(IEnumerable<IChannel> channels)
    {
        _defaultValues.AddRange(channels.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Channel)));
        return this;
    }

    internal override ChannelSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
