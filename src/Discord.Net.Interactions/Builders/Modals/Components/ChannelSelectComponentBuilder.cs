using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="ChannelSelectComponentInfo"/>.
/// </summary>
public class ChannelSelectComponentBuilder : SnowflakeSelectComponentBuilder<ChannelSelectComponentInfo, ChannelSelectComponentBuilder>
{
    private readonly List<ChannelType> _channelTypes = new();

    protected override ChannelSelectComponentBuilder Instance => this;

    /// <summary>
    ///     Gets the presented channel types for this Channel Select.
    /// </summary>
    public IReadOnlyCollection<ChannelType> ChannelTypes => _channelTypes.AsReadOnly();

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

    /// <summary>
    ///     Sets the value of <see cref="ChannelTypes"/>.
    /// </summary>
    /// <param name="channelTypes">the new value of <see cref="ChannelTypes"/>.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public ChannelSelectComponentBuilder WithChannelTypes(params IEnumerable<ChannelType> channelTypes)
    {
        _channelTypes.AddRange(channelTypes);
        return this;
    }

    internal override ChannelSelectComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
