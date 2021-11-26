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
        ///     Gets the flags that are relevant to this activity.
        /// </summary>
        /// <remarks>
        ///     This value is determined by bitwise OR-ing <see cref="ActivityProperties"/> values together.
        /// </remarks>
        /// <returns>
        ///     The value of flags for this activity.
        /// </returns>
        ActivityProperties Flags { get; }
        /// <summary>
        ///     Gets the details on what the player is currently doing.
        /// </summary>
        /// <returns>
        ///     A string describing what the player is doing.
        /// </returns>
        string Details { get; }
    }
}
