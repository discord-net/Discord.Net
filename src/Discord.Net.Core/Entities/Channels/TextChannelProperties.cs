using System;

namespace Discord
{
    /// <inheritdoc />
    public class TextChannelProperties : GuildChannelProperties
    {
        /// <summary>
        /// What the topic of the channel should be set to.
        /// </summary>
        public Optional<string> Topic { get; set; }
        /// <summary>
        /// Should this channel be flagged as NSFW?
        /// </summary>
        public Optional<bool> IsNsfw { get; set; }
        /// <summary>
        /// What the slow-mode ratelimit for this channel should be set to; 0 will disable slow-mode.
        /// </summary>
        /// <remarks>
        /// This value must fall within [0, 120]
        /// 
        /// Users with <see cref="ChannelPermission.ManageMessages"/> will be exempt from slow-mode.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Throws ArgummentOutOfRange if the value does not fall within [0, 120]</exception>
        public Optional<int> SlowModeInterval { get; set; }
    }
}
