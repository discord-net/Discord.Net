namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the status for an <see cref="User"/>.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// <see cref="User"/> is offline.
        /// </summary>
        Offline,

        /// <summary>
        /// <see cref="User"/> is online.
        /// </summary>
        Online,

        /// <summary>
        /// <see cref="User"/> is idle.
        /// </summary>
        Idle,

        /// <summary>
        /// <see cref="User"/> is on do not disturb.
        /// </summary>
        DoNotDisturb,
    }
}
