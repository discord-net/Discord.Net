using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the activity flags.
    /// </summary>
    [Flags]
    public enum ActivityFlags
    {
        /// <summary>
        ///     Activity instance.
        /// </summary>
        Instance = 1 << 0,

        /// <summary>
        ///     Activity join.
        /// </summary>
        Join = 1 << 1,

        /// <summary>
        ///     Activity spectate.
        /// </summary>
        Spectate = 1 << 2,

        /// <summary>
        ///     Activity join request.
        /// </summary>
        JoinRequest = 1 << 3,

        /// <summary>
        ///     Activity sync.
        /// </summary>
        Sync = 1 << 4,

        /// <summary>
        ///     Activity play.
        /// </summary>
        Play = 1 << 5,
    }
}
