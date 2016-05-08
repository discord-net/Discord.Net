using System;

namespace Discord
{
    public interface ISnowflakeEntity : IEntity<ulong>
    {
        /// <summary> Gets when this object was created. </summary>
        DateTime CreatedAt { get; }
    }
}
