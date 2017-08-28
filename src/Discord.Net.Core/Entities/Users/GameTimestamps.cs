using System;

namespace Discord
{
    public struct GameTimestamps
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