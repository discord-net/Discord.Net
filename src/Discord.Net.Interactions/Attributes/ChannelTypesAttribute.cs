using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Specify the target channel types for a <see cref="ApplicationCommandOptionType.Channel"/> option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class ChannelTypesAttribute : Attribute
    {
        /// <summary>
        ///     Gets the allowed channel types for this option.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

        /// <summary>
        ///     Specify the target channel types for a <see cref="ApplicationCommandOptionType.Channel"/> option.
        /// </summary>
        /// <param name="channelTypes">The allowed channel types for this option.</param>
        public ChannelTypesAttribute(params ChannelType[] channelTypes)
        {
            if (channelTypes is null)
                throw new ArgumentNullException(nameof(channelTypes));

            ChannelTypes = channelTypes.ToImmutableArray();
        }
    }
}
