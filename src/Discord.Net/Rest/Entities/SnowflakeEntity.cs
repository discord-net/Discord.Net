using System;

namespace Discord.Rest
{
    internal abstract class SnowflakeEntity : Entity<ulong>, ISnowflakeEntity
    {
        //TODO: C#7 Candidate for Extension Property. Lets us remove this class.
        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);

        public SnowflakeEntity(ulong id)
            : base(id)
        {
        }
    }
}
