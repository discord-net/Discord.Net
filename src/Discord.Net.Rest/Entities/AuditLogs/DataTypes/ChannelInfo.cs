namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a channel.
    /// </summary>
    public struct ChannelInfo
    {
        internal ChannelInfo(string name, string topic, int? rateLimit, bool? nsfw, int? bitrate, ChannelType? type)
        {
            Name = name;
            Topic = topic;
            SlowModeInterval = rateLimit;
            IsNsfw = nsfw;
            Bitrate = bitrate;
            ChannelType = type;
        }

        /// <summary>
        ///     Gets the name of this channel.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this channel.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the topic of this channel.
        /// </summary>
        /// <returns>
        ///     A string containing the topic of this channel, if any.
        /// </returns>
        public string Topic { get; }
        /// <summary>
        ///     Gets the current slow-mode delay of this channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds required before the user can send another
        ///     message; <c>0</c> if disabled.
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public int? SlowModeInterval { get; }
        /// <summary>
        ///     Gets the value that indicates whether this channel is NSFW.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this channel has the NSFW flag enabled; otherwise <c>false</c>.
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public bool? IsNsfw { get; }
        /// <summary>
        ///     Gets the bit-rate of this channel if applicable.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the bit-rate set for the voice channel;
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public int? Bitrate { get; }
        /// <summary>
        ///     Gets the type of this channel.
        /// </summary>
        /// <returns>
        ///     The channel type of this channel; <c>null</c> if not applicable.
        /// </returns>
        public ChannelType? ChannelType { get; }
    }
}
