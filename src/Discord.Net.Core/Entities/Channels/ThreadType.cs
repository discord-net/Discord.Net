namespace Discord
{
    /// <summary>
    ///     Represents types of threads.
    /// </summary>
    public enum ThreadType
    {
        /// <summary>
        ///     Represents a temporary sub-channel within a GUILD_NEWS channel.
        /// </summary>
        NewsThread = 10,

        /// <summary>
        ///     Represents a temporary sub-channel within a GUILD_TEXT channel.
        /// </summary>
        PublicThread = 11,

        /// <summary>
        ///     Represents a temporary sub-channel within a GUILD_TEXT channel that is only viewable by those invited and those with the MANAGE_THREADS permission
        /// </summary>
        PrivateThread = 12
    }
}
