using System;

namespace Discord.Extensions
{
    public static class SnowflakeEntityExtensions
    {
        //TODO: C#7 Candidate for extension property. 
        public static DateTimeOffset GetCreatedAt(this ISnowflakeEntity entity) => DateTimeUtils.FromSnowflake(entity.Id);
    }
}
