using System;

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="ITextChannel"/> with the specified changes.
    /// </summary>
    /// <seealso cref="ITextChannel.ModifyAsync(System.Action{TextChannelProperties}, RequestOptions)"/>
    public class TextChannelProperties : GuildChannelProperties
    {
        /// <summary>
        ///     Gets or sets the topic of the channel.
        /// </summary>
        /// <remarks>
        ///     Setting this value to any string other than <c>null</c> or <see cref="string.Empty"/> will set the
        ///     channel topic or description to the desired value.
        /// </remarks>
        public Optional<string> Topic { get; set; }
        /// <summary>
        ///     Gets or sets whether this channel should be flagged as NSFW.
        /// </summary>
        /// <remarks>
        ///     Setting this value to <c>true</c> will mark the channel as NSFW (Not Safe For Work) and will prompt the
        ///     user about its possibly mature nature before they may view the channel; setting this value to <c>false</c> will
        ///     remove the NSFW indicator.
        /// </remarks>
        public Optional<bool> IsNsfw { get; set; }
        /// <summary>
        ///     Gets or sets the slow-mode ratelimit in seconds for this channel.
        /// </summary>
        /// <remarks>
        ///     Setting this value to anything above zero will require each user to wait X seconds before
        ///     sending another message; setting this value to <c>0</c> will disable slow-mode for this channel.
        ///     <note>
        ///         Users with <see cref="Discord.ChannelPermission.ManageMessages"/> or 
        ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from slow-mode.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 21600].</exception>
        public Optional<int> SlowModeInterval { get; set; }

        /// <summary>
        ///     Gets or sets whether or not the thread is archived.
        /// </summary>
        public Optional<bool> Archived { get; set; }

        /// <summary>
        ///     Gets or sets whether or not the thread is locked.
        /// </summary>
        public Optional<bool> Locked { get; set; }

        /// <summary>
        ///     Gets or sets the auto archive duration.
        /// </summary>
        public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
        
    }
}
