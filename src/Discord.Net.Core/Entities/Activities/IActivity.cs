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
        ///     Gets the Stream URL. Only used when <see cref="Type"/> is <see cref="ActivityType.Streaming"/>.
        /// </summary>
        /// <returns>
        ///     A string containing the URL to the stream.
        /// </returns>
        string Url { get; }
        /// <summary>
        ///     Gets the unix timestamps for the start and/or end of the activity.
        /// </summary>
        /// <returns>
        ///     A <see cref="GameTimestamps"/> containing the start and end times, if specified.
        /// </returns>
        GameTimestamps Timestamps { get; }
        /// <summary>
        ///     Gets the application Id for the game.
        /// </summary>
        ulong ApplicationId { get; }
        /// <summary>
        ///     Gets what the user is currently doing.
        /// </summary>
        string Details { get; }
        /// <summary>
        ///     Gets the user's current party status.
        /// </summary>
        string State { get; }
        //TODO finish docs
        GameParty Party { get; }
        GameAsset Assets { get; }
        GameSecrets Secrets { get; }
        bool Instance { get; }
        int Flags { get; }
    }
}
