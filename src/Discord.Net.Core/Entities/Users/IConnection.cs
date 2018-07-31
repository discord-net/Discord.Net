using System.Collections.Generic;

namespace Discord
{
    public interface IConnection
    {
        /// <summary>
        ///     ID of the connection account.
        /// </summary>
        string Id { get; }
        /// <summary>
        ///     The service of the connection (twich, youtube).
        /// </summary>
        string Type { get; }
        /// <summary>
        ///     The username of the connection account.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Wheter the connection is revoked.
        /// </summary>
        bool IsRevoked { get; }

        /// <summary>
        ///     A <see cref="IReadOnlyCollection{T}"/> of integration IDs.
        /// </summary>
        IReadOnlyCollection<ulong> IntegrationIds { get; }
    }
}
