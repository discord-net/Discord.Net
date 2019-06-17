using System;

namespace Discord
{
    /// <summary>
    ///     Flags for the <see cref="IActivity.Flags"/> property, that are ORd together.
    ///     These describe what the activity payload includes.
    /// </summary>
    [Flags]
    public enum ActivityProperties
    {
        /// <summary>
        ///     Indicates that no actions on this activity can be taken.
        /// </summary>
        None = 0,
        Instance = 1,
        /// <summary>
        ///     Indicates that this activity can be joined.
        /// </summary>
        Join = 0b10,
        /// <summary>
        ///     Indicates that this activity can be spectated.
        /// </summary>
        Spectate = 0b100,
        /// <summary>
        ///     Indicates that a user may request to join an activity.
        /// </summary>
        JoinRequest = 0b1000,
        /// <summary>
        ///     Indicates that a user can listen along in Spotify.
        /// </summary>
        Sync = 0b10000,
        /// <summary>
        ///     Indicates that a user can play this song.
        /// </summary>
        Play = 0b100000
    }
}
