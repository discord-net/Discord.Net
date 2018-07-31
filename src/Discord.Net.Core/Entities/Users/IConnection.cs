using System.Collections.Generic;

namespace Discord
{
    public interface IConnection
    {
        /// <summary> Gets the ID of the connection account. </summary>
        /// <returns> Gets the ID of the connection account. </returns>
        string Id { get; }
        /// <summary> Gets the service of the connection (twitch, youtube). </summary>
        /// <returns> Gets the service of the connection (twitch, youtube). </returns>
        string Type { get; }
        /// <summary> Gets the username of the connection account. </summary>
        /// <returns> Gets the username of the connection account. </returns>
        string Name { get; }
        /// <summary> Gets whether the connection is revoked. </summary>
        /// <returns> Gets whether the connection is revoked. </returns>
        bool IsRevoked { get; }
        /// <summary> Gets a <see cref="IReadOnlyCollection{T}"/> of integration IDs. </summary>
        /// <returns> Gets a <see cref="IReadOnlyCollection{T}"/> of integration IDs. </returns>
        IReadOnlyCollection<ulong> IntegrationIds { get; }
    }
}
