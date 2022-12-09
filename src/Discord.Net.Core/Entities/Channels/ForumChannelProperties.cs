using System;
using System.Collections.Generic;

namespace Discord;

public class ForumChannelProperties : TextChannelProperties
{

    /// <summary>
    ///     Gets or sets the topic of the channel.
    /// </summary>
    /// <remarks>
    ///     Not available in forum channels.
    /// </remarks>
    public new Optional<int> SlowModeInterval { get; }

    /// <summary>
    /// Gets or sets rate limit on creating posts in this forum channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to anything above zero will require each user to wait X seconds before
    ///     creating another thread; setting this value to <c>0</c> will disable rate limits for this channel.
    ///     <note>
    ///         Users with <see cref="Discord.ChannelPermission.ManageMessages"/> or 
    ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from rate limits.
    ///     </note>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 21600].</exception>
    public Optional<int> ThreadCreationInterval { get; set; }


    /// <summary>
    /// Gets or sets the default slow-mode for threads in this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to anything above zero will require each user to wait X seconds before
    ///     sending another message; setting this value to <c>0</c> will disable slow-mode for child threads.
    ///     <note>
    ///         Users with <see cref="Discord.ChannelPermission.ManageMessages"/> or 
    ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from slow-mode.
    ///     </note>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 21600].</exception>
    public Optional<int> DefaultSlowModeInterval { get; set; }

    /// <summary>
    /// Gets or sets a collection of tags inside of this forum channel.
    /// </summary>
    public Optional<IEnumerable<ForumTagProperties>> Tags { get; set; }

    /// <summary>
    /// Gets or sets a new default reaction emoji in this forum channel.
    /// </summary>
    public Optional<IEmote> DefaultReactionEmoji { get; set; }

    /// <summary>
    /// Gets or sets the rule used to order posts in forum channels.
    /// </summary>
    public Optional<ForumSortOrder> DefaultSortOrder { get; set; }
}
