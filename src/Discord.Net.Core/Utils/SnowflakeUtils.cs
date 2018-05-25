using System;

namespace Discord
{
    public static class SnowflakeUtils
    {
        public static DateTimeOffset FromSnowflake(ulong value)
            => DateTimeOffset.FromUnixTimeMilliseconds((long)((value >> 22) + 1420070400000UL));
        public static ulong ToSnowflake(DateTimeOffset value)
            => ((ulong)value.ToUnixTimeMilliseconds() - 1420070400000UL) << 22;
    }
}
