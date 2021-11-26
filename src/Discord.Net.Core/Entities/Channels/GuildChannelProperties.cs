using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="IGuildChannel" /> with the specified changes.
    /// </summary>
    /// <seealso cref="IGuildChannel.ModifyAsync"/>
    public class GuildChannelProperties
    {
        /// <summary>
        ///     Gets or sets the channel to this name.
        /// </summary>
        /// <remarks>
        ///     This property defines the new name for this channel.
        ///     <note type="warning">
        ///         When modifying an <see cref="ITextChannel"/>, the <see cref="Name"/> must be alphanumeric with
        ///         dashes. It must match the RegEx <c>[a-z0-9-_]{2,100}</c>.
        ///     </note>
        /// </remarks>
        public Optional<string> Name { get; set; }
        /// <summary>
        ///     Moves the channel to the following position. This property is zero-based.
        /// </summary>
        public Optional<int> Position { get; set; }
        /// <summary>
        ///     Gets or sets the category ID for this channel.
        /// </summary>
        /// <remarks>
        ///     Setting this value to a category's snowflake identifier will change or set this channel's parent to the
        ///     specified channel; setting this value to <see langword="null"/> will detach this channel from its parent if one
        ///     is set.
        /// </remarks>
        public Optional<ulong?> CategoryId { get; set; }
        /// <summary>
        ///     Gets or sets the permission overwrites for this channel.
        /// </summary>
        public Optional<IEnumerable<Overwrite>> PermissionOverwrites { get; set; }
    }
}
