using System;

namespace Discord
{
    internal static class DateTimeUtils
    {
#if !NETSTANDARD1_3
        //https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/DateTimeOffset.cs
        private const long UnixEpochTicks = 621355968000000000;
        private const long UnixEpochSeconds = 62135596800;
#endif

        public static DateTimeOffset FromSnowflake(ulong value)
            => FromUnixMilliseconds((long)((value >> 22) + 1420070400000UL));

        public static DateTimeOffset FromTicks(long ticks)
            => new DateTimeOffset(ticks, TimeSpan.Zero);
        public static DateTimeOffset? FromTicks(long? ticks)
            => ticks != null ? new DateTimeOffset(ticks.Value, TimeSpan.Zero) : (DateTimeOffset?)null;

        public static DateTimeOffset FromUnixSeconds(long seconds)
        {
#if NETSTANDARD1_3
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
#else
            long ticks = seconds * TimeSpan.TicksPerSecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }
        public static DateTimeOffset FromUnixMilliseconds(long seconds)
        {
#if NETSTANDARD1_3
            return DateTimeOffset.FromUnixTimeMilliseconds(seconds);
#else
            long ticks = seconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }

        public static long ToUnixSeconds(DateTimeOffset dto)
        {
#if NETSTANDARD1_3
            return dto.ToUnixTimeSeconds();
#else
            long seconds = dto.UtcDateTime.Ticks / TimeSpan.TicksPerSecond;
            return seconds - UnixEpochSeconds;
#endif
        }
        public static long ToUnixMilliseconds(DateTimeOffset dto)
        {
#if NETSTANDARD1_3
            return dto.ToUnixTimeMilliseconds();
#else
            long seconds = dto.UtcDateTime.Ticks / TimeSpan.TicksPerMillisecond;
            return seconds - UnixEpochSeconds;
#endif
        }
    }
}
