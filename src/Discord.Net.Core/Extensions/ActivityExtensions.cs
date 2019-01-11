namespace Discord
{
    /// <summary>
    ///     Extension methods for the <see cref="IActivity"/> types.
    /// </summary>
    public static class ActivityExtensions
    {
        /// <summary>
        ///     Determines if the given flag is enabled with this activity.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="flag">The <see cref="ActivityFlag"/> to check for.</param>
        /// <returns>
        ///     True if the activity has this flag enabled, false otherwise.
        /// </returns>
        public static bool CheckActivityFlag(this IActivity activity, ActivityFlag flag)
            => (activity.Flags & (int)flag) >= (int)flag;
    }
}
