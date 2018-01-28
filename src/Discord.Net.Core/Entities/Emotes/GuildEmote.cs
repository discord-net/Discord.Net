using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    /// An image-based emote that is attached to a guild
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class GuildEmote : Emote
    {
        public bool IsManaged { get; }
        public bool RequireColons { get; }
        public IReadOnlyList<ulong> RoleIds { get; }

        internal GuildEmote(ulong id, string name, bool animated, bool isManaged, bool requireColons, IReadOnlyList<ulong> roleIds) : base(id, name, animated)
        {
            IsManaged = isManaged;
            RequireColons = requireColons;
            RoleIds = roleIds;
        }

        private string DebuggerDisplay => $"{Name} ({Id})";
        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
