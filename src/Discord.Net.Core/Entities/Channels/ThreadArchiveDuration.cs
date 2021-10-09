namespace Discord
{
    /// <summary>
    ///     Represents the thread auto archive duration.
    /// </summary>
    public enum ThreadArchiveDuration
    {
        /// <summary>
        ///     One hour (60 minutes).
        /// </summary>
        OneHour = 60,

        /// <summary>
        ///     One day (1440 minutes).
        /// </summary>
        OneDay = 1440,

        /// <summary>
        ///     Three days (4320 minutes).
        ///     <remarks>
        ///         This option is explicitly available to nitro users.
        ///     </remarks>
        /// </summary>
        ThreeDays = 4320,

        /// <summary>
        ///     One week (10080 minutes).
        ///     <remarks>
        ///         This option is explicitly available to nitro users.
        ///     </remarks>
        /// </summary>
        OneWeek = 10080
    }
}
