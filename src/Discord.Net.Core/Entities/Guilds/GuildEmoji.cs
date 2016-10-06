using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Discord.API.Emoji;

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

        private GuildEmoji(ulong id, string name, bool isManaged, bool requireColons, IReadOnlyList<ulong> roleIds)
        {
            Id = id;
            Name = name;
            IsManaged = isManaged;
            RequireColons = requireColons;
            RoleIds = roleIds;
        }
        internal static GuildEmoji Create(Model model)
        {
            return new GuildEmoji(model.Id, model.Name, model.Managed, model.RequireColons, ImmutableArray.Create(model.Roles));
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
