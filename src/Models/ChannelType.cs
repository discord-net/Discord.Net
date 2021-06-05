namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the type of a <see cref="Channel"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#channel-object-channel-types"/>
    /// </remarks>
    public enum ChannelType
    {
        /// <summary>
        /// A text channel within a server.
        /// </summary>
        GuildText = 0,

        /// <summary>
        /// A direct message between users.
        /// </summary>
        DM = 1,

        /// <summary>
        /// A voice channel within a server.
        /// </summary>
        GuildVoice = 2,

        /// <summary>
        /// A direct message between multiple users.
        /// </summary>
        GroupDM = 3,

        /// <summary>
        /// An organizational category that contains up to 50 channels.
        /// </summary>
        GuildCategory = 4,

        /// <summary>
        /// A channel that users can follow and crosspost into their own server.
        /// </summary>
        GuildNews = 5,

        /// <summary>
        /// A channel in which game developers can sell their game on Discord.
        /// </summary>
        GuildStore = 6,

        /// <summary>
        /// A temporary sub-channel within a <see cref="GuildNews"/> channel.
        /// </summary>
        GuildNewsThread = 10,

        /// <summary>
        /// A temporary sub-channel within a <see cref="GuildText"/> channel.
        /// </summary>
        GuildPublicThread = 11,

        /// <summary>
        /// A temporary sub-channel within a <see cref="GuildText"/> channel
        /// that is only viewable by those invited and those with the
        /// MANAGE_THREADS permission.
        /// </summary>
        GuildPrivateThread = 12,

        /// <summary>
        /// A voice channel for hosting events with an audience.
        /// </summary>
        GuildStageVoice = 13
    }
}
