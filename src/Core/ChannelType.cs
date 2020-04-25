namespace Discord.Core
{
    /// <summary>
    /// The type of channel a <see cref="IChannel"/> is.
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// A guild text channel.
        /// </summary>
        GuildText = 0,
        /// <summary>
        /// A direct message channel.
        /// </summary>
        DirectMessage = 1,
        /// <summary>
        /// A guild voice channel.
        /// </summary>
        GuildVoice = 2,
        /// <summary>
        /// A group direct message channel.
        /// </summary>
        GroupDirectMessage = 3,
        /// <summary>
        /// A guild category channel.
        /// </summary>
        GuildCategory = 4,
        /// <summary>
        /// A guild news channel.
        /// </summary>
        GuildNews = 5,
        /// <summary>
        /// A guild store channel.
        /// </summary>
        GuildStore = 6,
    }
}
