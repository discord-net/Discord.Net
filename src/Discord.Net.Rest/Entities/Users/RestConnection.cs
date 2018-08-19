using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Discord.API.Connection;

namespace Discord
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class RestConnection : IConnection
    {
        internal RestConnection(string id, string type, string name, bool isRevoked,
            IReadOnlyCollection<ulong> integrationIds)
        {
            Id = id;
            Type = type;
            Name = name;
            IsRevoked = isRevoked;

            IntegrationIds = integrationIds;
        }

        private string DebuggerDisplay => $"{Name} ({Id}, {Type}{(IsRevoked ? ", Revoked" : "")})";
        public string Id { get; }
        public string Type { get; }
        public string Name { get; }
        public bool IsRevoked { get; }
        public IReadOnlyCollection<ulong> IntegrationIds { get; }

        internal static RestConnection Create(Model model) => new RestConnection(model.Id, model.Type, model.Name,
            model.Revoked, model.Integrations.ToImmutableArray());

        public override string ToString() => Name;
    }
}
