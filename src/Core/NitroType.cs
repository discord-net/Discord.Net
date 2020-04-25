namespace Discord.Core
{
    /// <summary>
    /// The type of Nitro subscription which a <see cref="IUser"/> may have.
    /// </summary>
    public enum NitroType
    {
        /// <summary>
        /// The user has no nitro subscription.
        /// </summary>
        None = 0,
        /// <summary>
        /// The user has Nitro Classic.
        /// </summary>
        Classic = 1,
        /// <summary>
        /// The user has Discord Nitro.
        /// </summary>
        Nitro = 2,
    }
}
