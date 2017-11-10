using System;

namespace Discord
{
    public class GameTimestamps
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }

        public GameTimestamps(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
        }
    }
}