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
        string Id { get; }
        /// <summary>
        ///     Gets the name of this voice region.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Returns <see langword="true"/> if this voice region is exclusive to VIP accounts.
        /// </summary>
        bool IsVip { get; }
        /// <summary>
        ///     Returns <see langword="true"/> if this voice region is the closest to your machine.
        /// </summary>
        bool IsOptimal { get; }
        /// <summary> 
        ///     Returns <see langword="true"/> if this is a deprecated voice region (avoid switching to these).
        /// </summary>
        bool IsDeprecated { get; }
        /// <summary> 
        ///     Returns <see langword="true"/> if this is a custom voice region (used for events/etc).
        /// </summary>
        bool IsCustom { get; }
    }
}
