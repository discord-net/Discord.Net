namespace Discord
{
    /// <summary>
    ///     Represents a generic ban object.
    /// </summary>
    public interface IBan
    {
        /// <summary>
        ///     Gets the banned user.
        /// </summary>
        /// <returns>
        ///     A user that was banned.
        /// </returns>
        IUser User { get; }
        /// <summary>
        ///     Gets the reason why the user is banned if specified.
        /// </summary>
        /// <returns>
        ///     A string containing the reason behind the ban; <c>null</c> if none is specified.
        /// </returns>
        string Reason { get; }
    }
}
