using System;

namespace Discord
{
    public static class SnowflakeUtils
    {
        public static DateTimeOffset FromSnowflake(ulong value)
            => DateTimeUtils.FromUnixMilliseconds((long)((value >> 22) + 1420070400000UL));
        public static ulong ToSnowflake(DateTimeOffset value)
            => ((ulong)DateTimeUtils.ToUnixMilliseconds(value) - 1420070400000UL) << 22;
    }
}
