using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="ChannelSelectInputComponentInfo"/>.
/// </summary>
public class ChannelSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<ChannelSelectInputComponentInfo, ChannelSelectInputComponentBuilder>
{
    protected override ChannelSelectInputComponentBuilder Instance => this;

    /// <summary>
    ///     Initializes a new <see cref="ChannelSelectInputComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this component.</param>
    public ChannelSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.ChannelSelect) { }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channel">The channel to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectInputComponentBuilder AddDefaulValue(IChannel channel)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channel.Id, SelectDefaultValueType.Channel));
        return this;
    }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channelId">The channel ID to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectInputComponentBuilder AddDefaulValue(ulong channelId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channelId, SelectDefaultValueType.Channel));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channels">The channels to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectInputComponentBuilder AddDefaultValues(params IChannel[] channels)
    {
        _defaultValues.AddRange(channels.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Channel)));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channels">The channels to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectInputComponentBuilder AddDefaultValues(IEnumerable<IChannel> channels)
    {
        _defaultValues.AddRange(channels.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Channel)));
        return this;
    }

    internal override ChannelSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
