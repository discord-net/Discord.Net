using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="ChannelSelectComponentInfo"/>.
/// </summary>
public class ChannelSelectComponentBuilder : SnowflakeSelectComponentBuilder<ChannelSelectComponentInfo, ChannelSelectComponentBuilder>
{
    protected override ChannelSelectComponentBuilder Instance => this;

    /// <summary>
    ///     Initializes a new <see cref="ChannelSelectComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this component.</param>
    public ChannelSelectComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.ChannelSelect) { }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channelId">The channel ID to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectComponentBuilder AddDefaulValue(ulong channelId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channelId, SelectDefaultValueType.Channel));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channels">The channels to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectComponentBuilder AddDefaultValues(params IEnumerable<IChannel> channels)
    {
        _defaultValues.AddRange(channels.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Channel)));
        return this;
    }

    internal override ChannelSelectComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
