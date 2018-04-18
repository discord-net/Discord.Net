using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     An image-based emote that is attached to a guild.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class GuildEmote : Emote
    {
        /// <summary>
        ///     Gets whether this emoji is managed.
        /// </summary>
        public bool IsManaged { get; }
        /// <summary>
        ///     Gets whether this emoji must be wrapped in colons.
        /// </summary>
        public bool RequireColons { get; }
        /// <summary>
        ///     Gets the roles this emoji is whitelisted to.
        /// </summary>
        public IReadOnlyList<ulong> RoleIds { get; }

        internal GuildEmote(ulong id, string name, bool animated, bool isManaged, bool requireColons, IReadOnlyList<ulong> roleIds) : base(id, name, animated)
        {
            IsManaged = isManaged;
            RequireColons = requireColons;
            RoleIds = roleIds;
        }

        private string DebuggerDisplay => $"{Name} ({Id})";
        /// <summary>
        ///     Gets the raw representation of the emoji.
        /// </summary>
        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
