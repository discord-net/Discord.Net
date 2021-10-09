namespace Discord
{
    /// <summary>
    ///     Specifies the privacy levels of a Stage instance.
    /// </summary>
    public enum StagePrivacyLevel
    {
        /// <summary>
        ///     The Stage instance is visible publicly, such as on Stage Discovery.
        /// </summary>
        Public = 1,
        /// <summary>
        ///     The Stage instance is visible to only guild members.
        /// </summary>
        GuildOnly = 2
    }
}
