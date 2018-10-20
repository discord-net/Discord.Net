namespace Discord
{
    /// <summary>
    ///     Represents a region of which the user connects to when using voice.
    /// </summary>
    public interface IVoiceRegion
    {
        /// <summary>
        ///     Gets the unique identifier for this voice region.
        /// </summary>
        /// <returns>
        ///     A string that represents the identifier for this voice region (e.g. <c>eu-central</c>).
        /// </returns>
        string Id { get; }
        /// <summary>
        ///     Gets the name of this voice region.
        /// </summary>
        /// <returns>
        ///     A string that represents the human-readable name of this voice region (e.g. <c>Central Europe</c>).
        /// </returns>
        string Name { get; }
        /// <summary>
        ///     Gets a value that indicates whether or not this voice region is exclusive to partnered servers.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this voice region is exclusive to VIP accounts; otherwise <c>false</c>.
        /// </returns>
        bool IsVip { get; }
        /// <summary>
        ///     Gets a value that indicates whether this voice region is optimal for your client in terms of latency.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this voice region is the closest to your machine; otherwise <c>false</c> .
        /// </returns>
        bool IsOptimal { get; }
        /// <summary>
        ///     Gets a value that indicates whether this voice region is no longer being maintained.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this is a deprecated voice region; otherwise <c>false</c>.
        /// </returns>
        bool IsDeprecated { get; }
        /// <summary>
        ///     Gets a value that indicates whether this voice region is custom-made for events.
        /// </summary>
        /// <returns> 
        ///     <c>true</c> if this is a custom voice region (used for events/etc); otherwise <c>false</c>/
        /// </returns>
        bool IsCustom { get; }
    }
}
