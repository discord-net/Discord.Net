using System;

namespace Discord
{
    internal static class DateTimeUtils
    {
        private const ulong EpochTicks = 621355968000000000UL;
        private const ulong DiscordEpochMillis = 1420070400000UL;

        public static DateTime FromEpochMilliseconds(ulong value)
            => new DateTime((long)(value * TimeSpan.TicksPerMillisecond + EpochTicks), DateTimeKind.Utc);
        public static DateTime FromEpochSeconds(ulong value)
            => new DateTime((long)(value * TimeSpan.TicksPerSecond + EpochTicks), DateTimeKind.Utc);

        public static DateTime FromSnowflake(ulong value)
            => FromEpochMilliseconds((value >> 22) + DiscordEpochMillis);
    }
}
