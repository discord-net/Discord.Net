using System.Collections.Generic;
using System.Diagnostics;
using Model = Discord.API.Connection;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class Connection : IConnection
    {
        public string Id { get; }
        public string Type { get; }
        public string Name { get; }
        public bool IsRevoked { get; }

        public IReadOnlyCollection<ulong> IntegrationIds { get; }

        public Connection(Model model)
        {
            Id = model.Id;
            Type = model.Type;
            Name = model.Name;
            IsRevoked = model.Revoked;

            IntegrationIds = model.Integrations;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, Type = {Type}{(IsRevoked ? ", Revoked" : "")})";
    }
}
