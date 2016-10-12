using System;

namespace Discord
{
    public interface ISnowflakeEntity : IEntity<ulong>
    {
        DateTimeOffset CreatedAt { get; }
    }
}
