using System.Collections.Generic;

namespace Discord
{
    public interface IConnection
    {
        /// <summary> Gets the ID of the connection account. </summary>
        /// <returns> A <see cref="string"/> representing the unique identifier value of this connection. </returns>
        string Id { get; }
        /// <summary> Gets the service of the connection (twitch, youtube). </summary>
        /// <returns> A string containing the name of this type of connection. </returns>
        string Type { get; }
        /// <summary> Gets the username of the connection account. </summary>
        /// <returns> A string containing the name of this connection. </returns>
        string Name { get; }
        /// <summary> Gets whether the connection is revoked. </summary>
        /// <returns> A value which if true indicates that this connection has been revoked, otherwise false. </returns>
        bool IsRevoked { get; }

        /// <summary> Gets a <see cref="IReadOnlyCollection{T}"/> of integration IDs. </summary>
        /// <returns>
        ///     An <see cref="IReadOnlyCollection{T}"/> containing <see cref="ulong"/>
        ///     representations of unique identifier values of integrations.
        /// </returns>
        IReadOnlyCollection<ulong> IntegrationIds { get; }
    }
}
