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
        IUser User { get; }
        /// <summary>
        ///     Gets the reason why the user is banned.
        /// </summary>
        string Reason { get; }
    }
}
