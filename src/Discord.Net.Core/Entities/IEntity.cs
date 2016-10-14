using System;

namespace Discord
{
    public interface IEntity<TId>
        where TId : IEquatable<TId>
    {
        ///// <summary> Gets the IDiscordClient that created this object. </summary>
        //IDiscordClient Discord { get; }

        /// <summary> Gets the unique identifier for this object. </summary>
        TId Id { get; }

    }
}
