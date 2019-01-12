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
        /// <summary>
        ///     The flags that are relevant to this activity.
        /// </summary>
        /// <remarks>
        ///     This value is determined by bitwise OR-ing <see cref="ActivityFlag"/> values together.
        /// </remarks>
        /// <returns>
        ///     The value of flags for this activity.
        /// </returns>
        ActivityFlag Flags { get; }
    }
}
