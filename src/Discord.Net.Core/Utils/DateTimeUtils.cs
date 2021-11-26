using System;

namespace Discord
{
    /// <see href="https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/DateTimeOffset.cs"/>
    internal static class DateTimeUtils
    {
        public static DateTimeOffset FromTicks(long ticks)
            => new DateTimeOffset(ticks, TimeSpan.Zero);
        public static DateTimeOffset? FromTicks(long? ticks)
            => ticks != null ? new DateTimeOffset(ticks.Value, TimeSpan.Zero) : (DateTimeOffset?)null;
    }
}
