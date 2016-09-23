using System.Collections.Generic;

namespace Discord
{
    public interface IConnection
    {
        string Id { get; }
        string Type { get; }
        string Name { get; }
        bool IsRevoked { get; }

        IReadOnlyCollection<ulong> IntegrationIds { get; }
    }
}
