namespace Discord
{
    /// <summary>
    ///     A user's activity status, typically a <see cref="Game"/>.
    /// </summary>
    public interface IActivity
    {
        /// <summary>
        ///     Gets the name of the activity.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the activity that the user is doing.
        /// </returns>
        string Name { get; }
        /// <summary>
        ///     Gets the type of the activity.
        /// </summary>
        /// <returns>
        ///     The type of activity.
        /// </returns>
        ActivityType Type { get; }
    }
}
