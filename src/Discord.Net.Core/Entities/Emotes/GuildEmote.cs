using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     An image-based emote that is attached to a guild
    /// </summary>
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class GuildEmote : Emote
    {
        internal GuildEmote(ulong id, string name, bool animated, bool isManaged, bool requireColons,
            IReadOnlyList<ulong> roleIds) : base(id, name, animated)
        {
            IsManaged = isManaged;
            RequireColons = requireColons;
            RoleIds = roleIds;
        }

        public bool IsManaged { get; }
        public bool RequireColons { get; }
        public IReadOnlyList<ulong> RoleIds { get; }

        private string DebuggerDisplay => $"{Name} ({Id})";
        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
