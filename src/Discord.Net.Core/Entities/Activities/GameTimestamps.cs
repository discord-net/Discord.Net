using System;

namespace Discord
{
    /// <summary>
    ///     Timestamps for a <see cref="RichGame" /> object.
    /// </summary>
    public class GameTimestamps
    {
        /// <summary>
        ///     Gets when the activity started.
        /// </summary>
        public DateTimeOffset? Start { get; }
        /// <summary>
        ///     Gets when the activity ends.
        /// </summary>
        public DateTimeOffset? End { get; }

        internal GameTimestamps(DateTimeOffset? start, DateTimeOffset? end)
        {
            Start = start;
            End = end;
        }
    }
}
