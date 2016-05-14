using System.Collections.Generic;
using Model = Discord.API.Connection;

namespace Discord.Rest
{
    public class Connection : IConnection
    {
        public string Id { get; }

        public string Type { get; private set; }
        public string Name { get; private set; }
        public bool IsRevoked { get; private set; }

        public IEnumerable<ulong> Integrations { get; private set; }

        public Connection(Model model)
        {
            Id = model.Id;

            Type = model.Type;
            Name = model.Name;
            IsRevoked = model.Revoked;

            Integrations = model.Integrations;
        }

        public override string ToString() => $"{Name ?? Id.ToString()} ({Type})";
    }
}
