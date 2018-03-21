using System;

namespace Discord
{
    /// <summary> The timestamps for a <see cref="RichGame"/> object. </summary>
    public class GameTimestamps
    {
        public DateTimeOffset? Start { get; }
        public DateTimeOffset? End { get; }

        internal GameTimestamps(DateTimeOffset? start, DateTimeOffset? end)
        {
            Start = start;
            End = end;
        }
    }
}
