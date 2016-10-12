using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Discord.API.Connection;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestConnection : IConnection
    {
        public string Id { get; }
        public string Type { get; }
        public string Name { get; }
        public bool IsRevoked { get; }
        public IReadOnlyCollection<ulong> IntegrationIds { get; }

        internal RestConnection(string id, string type, string name, bool isRevoked, IReadOnlyCollection<ulong> integrationIds)
        {
            Id = id;
            Type = type;
            Name = name;
            IsRevoked = isRevoked;

            IntegrationIds = integrationIds;
        }
        internal static RestConnection Create(Model model)
        {
            return new RestConnection(model.Id, model.Type, model.Name, model.Revoked, model.Integrations.ToImmutableArray());
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, {Type}{(IsRevoked ? ", Revoked" : "")})";
    }
}
