namespace Discord
{
    public class SessionStartLimit
    {
        /// <summary>
        ///     The total number of session starts the current user is allowed.
        /// </summary>
        public int Total { get; internal set; }
        /// <summary>
        ///     The remaining number of session starts the current user is allowed.
        /// </summary>
        public int Remaining { get; internal set; }
        /// <summary>
        ///     The number of milliseconds after which the limit resets.
        /// </summary>
        public int ResetAfter { get; internal set; }
        /// <summary>
        ///     The maximum concurrent identify requests in a time window.
        /// </summary>
        public int MaxConcurrency { get; internal set; }
    }
}
