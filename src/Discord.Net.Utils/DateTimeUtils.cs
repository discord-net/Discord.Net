using System;

namespace Discord
{
    internal static class DateTimeUtils
    {
        public static DateTimeOffset FromSnowflake(ulong value)
            => DateTimeOffset.FromUnixTimeMilliseconds((long)((value >> 22) + 1420070400000UL));

        public static DateTimeOffset FromTicks(long ticks)
            => new DateTimeOffset(ticks, TimeSpan.Zero);
        public static DateTimeOffset? FromTicks(long? ticks)
            => ticks != null ? new DateTimeOffset(ticks.Value, TimeSpan.Zero) : (DateTimeOffset?)null;
    }
}
