namespace Discord
{
    /// <summary> Defines the types of channels. </summary>
    public enum ChannelType
    {
        /// <summary> The channel is a text channel. </summary>
        Text = 0,
        /// <summary> The channel is a Direct Message channel. </summary>
        DM = 1,
        /// <summary> The channel is a voice channel. </summary>
        Voice = 2,
        /// <summary> The channel is a group channel. </summary>
        Group = 3,
        /// <summary> The channel is a category channel. </summary>
        Category = 4,
        /// <summary> The channel is a news channel. </summary>
        News = 5,
        /// <summary> The channel is a store channel. </summary>
        Store = 6,
        /// <summary> The channel is a temporary thread channel under a news channel. </summary>
        NewsThread = 10,
        /// <summary> The channel is a temporary thread channel under a text channel.  </summary>
        PublicThread = 11,
        /// <summary> The channel is a private temporary thread channel under a text channel.  </summary>
        PrivateThread = 12,
        /// <summary> The channel is a stage voice channel. </summary>
        Stage = 13,
        /// <summary> The channel is a guild directory used in hub servers. (Unreleased)</summary>
        GuildDirectory = 14,
        /// <summary> The channel is a forum channel containing multiple threads. </summary>
        Forum = 15
    }
}
