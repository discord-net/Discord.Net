using System;

namespace Discord
{
    /// <summary> Represents a Discord snowflake entity. </summary>
    public interface ISnowflakeEntity : IEntity<ulong>
    {
        DateTimeOffset CreatedAt { get; }
    }
}
