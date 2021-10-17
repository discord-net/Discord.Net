namespace Discord
{
    /// <summary>
    ///     Represents a set of styles to use with a <see cref="TimestampTag"/>
    /// </summary>
    public enum TimestampTagStyles
    {
        /// <summary>
        ///     A short time string: 16:20
        /// </summary>
        ShortTime = 116,

        /// <summary>
        ///     A long time string: 16:20:30
        /// </summary>
        LongTime = 84,

        /// <summary>
        ///     A short date string: 20/04/2021
        /// </summary>
        ShortDate = 100,

        /// <summary>
        ///     A long date string: 20 April 2021
        /// </summary>
        LongDate = 68,

        /// <summary>
        ///     A short datetime string: 20 April 2021 16:20
        /// </summary>
        ShortDateTime = 102,

        /// <summary>
        ///     A long datetime string: Tuesday, 20 April 2021 16:20
        /// </summary>
        LongDateTime = 70,

        /// <summary>
        ///     The relative time to the user: 2 months ago
        /// </summary>
        Relative = 82
    }
}
