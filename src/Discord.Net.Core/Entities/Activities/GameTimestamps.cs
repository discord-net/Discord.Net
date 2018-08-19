using System;

namespace Discord
{
    public class GameTimestamps
    {
        internal GameTimestamps(DateTimeOffset? start, DateTimeOffset? end)
        {
            Start = start;
            End = end;
        }

        public DateTimeOffset? Start { get; }
        public DateTimeOffset? End { get; }
    }
}
