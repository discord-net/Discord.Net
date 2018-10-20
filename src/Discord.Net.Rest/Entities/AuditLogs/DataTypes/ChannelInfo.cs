namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a channel.
    /// </summary>
    public struct ChannelInfo
    {
        internal ChannelInfo(string name, string topic, int? bitrate, int? limit)
        {
            Name = name;
            Topic = topic;
            Bitrate = bitrate;
            UserLimit = limit;
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
        ///     Gets the bit-rate of this channel if applicable.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Int32"/> representing the bit-rate set for the voice channel; <c>null</c> if not
        ///     applicable.
        /// </returns>
        public int? Bitrate { get; }
        /// <summary>
        ///     Gets the number of users allowed to be in this channel if applicable.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Int32" /> representing the number of users allowed to be in this voice channel; 
        ///     <c>null</c> if not applicable.
        /// </returns>
        public int? UserLimit { get; }
    }
}
