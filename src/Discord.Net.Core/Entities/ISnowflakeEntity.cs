using System;

namespace Discord
{
    /// <summary> Represents a Discord snowflake entity. </summary>
    public interface ISnowflakeEntity : IEntity<ulong>
    {
        /// <summary> Gets when the snowflake is created. </summary>
        DateTimeOffset CreatedAt { get; }
    }
}
