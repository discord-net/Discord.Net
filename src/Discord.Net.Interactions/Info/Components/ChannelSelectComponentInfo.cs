using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions;

/// <summary>
///     Represents the <see cref="InputComponentInfo"/> class for <see cref="ComponentType.ChannelSelect"/> type.
/// </summary>
public class ChannelSelectComponentInfo : SnowflakeSelectComponentInfo
{
    /// <summary>
    ///     Gets the presented channel types for this Channel Select.
    /// </summary>
    public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

    internal ChannelSelectComponentInfo(Builders.ChannelSelectComponentBuilder builder, ModalInfo modal)
        : base(builder, modal)
    {
        ChannelTypes = builder.ChannelTypes.ToImmutableArray();
    }
}
