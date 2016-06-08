using System;

namespace Discord
{
    internal abstract class SnowflakeEntity : Entity<ulong>, ISnowflakeEntity
    {
        //TODO: Candidate for Extension Property. Lets us remove this class.
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);

        public SnowflakeEntity(ulong id)
            : base(id)
        {
        }
    }
}
