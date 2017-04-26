using System;

namespace Discord
{
    //Source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/DateTimeOffset.cs
    internal static class DateTimeUtils
    {
#if !UNIXTIME
        private const long UnixEpochTicks = 621_355_968_000_000_000;
        private const long UnixEpochSeconds = 62_135_596_800;
        private const long UnixEpochMilliseconds = 62_135_596_800_000;
#endif

        public static DateTimeOffset FromTicks(long ticks)
            => new DateTimeOffset(ticks, TimeSpan.Zero);
        public static DateTimeOffset? FromTicks(long? ticks)
            => ticks != null ? new DateTimeOffset(ticks.Value, TimeSpan.Zero) : (DateTimeOffset?)null;

        public static DateTimeOffset FromUnixSeconds(long seconds)
        {
#if UNIXTIME
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
#else
            long ticks = seconds * TimeSpan.TicksPerSecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }
        public static DateTimeOffset FromUnixMilliseconds(long milliseconds)
        {
#if UNIXTIME
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
#else
            long ticks = milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }

        public static long ToUnixSeconds(DateTimeOffset dto)
        {
#if UNIXTIME
            return dto.ToUnixTimeSeconds();
#else
            long seconds = dto.UtcDateTime.Ticks / TimeSpan.TicksPerSecond;
            return seconds - UnixEpochSeconds;
#endif
        }
        public static long ToUnixMilliseconds(DateTimeOffset dto)
        {
#if UNIXTIME
            return dto.ToUnixTimeMilliseconds();
#else
            long milliseconds = dto.UtcDateTime.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds - UnixEpochMilliseconds;
#endif
        }
    }
}
