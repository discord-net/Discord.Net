namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the guild nsfw level for a <see cref="Guild"/>.
    /// </summary>
    public enum GuildNsfwLevel
    {
        /// <summary>
        /// Default level.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Guild has explicit content.
        /// </summary>
        Explicit = 1,

        /// <summary>
        /// Guild is safe for work.
        /// </summary>
        Safe = 2,

        /// <summary>
        /// Guild is age-restricted.
        /// </summary>
        AgeRestricted = 3,
    }
}
