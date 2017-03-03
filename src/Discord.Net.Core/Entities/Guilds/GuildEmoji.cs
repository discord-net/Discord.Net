using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct GuildEmoji
    {
        public ulong Id { get; }
        public string Name { get; }
        public bool IsManaged { get; }
        public bool RequireColons { get; }
        public IReadOnlyList<ulong> RoleIds { get; }

        internal GuildEmoji(ulong id, string name, bool isManaged, bool requireColons, IReadOnlyList<ulong> roleIds)
        {
            Id = id;
            Name = name;
            IsManaged = isManaged;
            RequireColons = requireColons;
            RoleIds = roleIds;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
