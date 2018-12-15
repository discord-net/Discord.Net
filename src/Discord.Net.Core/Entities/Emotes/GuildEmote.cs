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
        ///     Gets whether this emoji is managed by an integration.
        /// </summary>
        /// <returns>
        ///     A boolean that determines whether or not this emote is managed by a Twitch integration.
        /// </returns>
        public bool IsManaged { get; }
        /// <summary>
        ///     Gets whether this emoji must be wrapped in colons.
        /// </summary>
        /// <returns>
        ///     A boolean that determines whether or not this emote requires the use of colons in chat to be used.
        /// </returns>
        public bool RequireColons { get; }
        /// <summary>
        ///     Gets the roles that are allowed to use this emoji.
        /// </summary>
        /// <returns>
        ///     A read-only list containing snowflake identifiers for roles that are allowed to use this emoji.
        /// </returns>
        public IReadOnlyList<ulong> RoleIds { get; }
        /// <summary>
        ///     Gets the cached User that created this emoji.
        /// </summary>
        /// <returns>
        ///     An optional <see cref="Cacheable{TEntity, TId}"/> <see cref="IUser"/> who created this emoji.
        ///     An unspecified value only indicates that the creator was not supplied as part of the API response.
        /// </returns>        
        public Optional<Cacheable<IUser, ulong>> Creator { get; }

        internal GuildEmote(ulong id, string name, bool animated, bool isManaged, bool requireColons, IReadOnlyList<ulong> roleIds, Optional<Cacheable<IUser, ulong>> creator) : base(id, name, animated)
        {
            IsManaged = isManaged;
            RequireColons = requireColons;
            RoleIds = roleIds;
            Creator = creator;
        }

        private string DebuggerDisplay => $"{Name} ({Id})";
        /// <summary>
        ///     Gets the raw representation of the emote.
        /// </summary>
        /// <returns>
        ///     A string representing the raw presentation of the emote (e.g. <c>&lt;:thonkang:282745590985523200&gt;</c>).
        /// </returns>
        public override string ToString() => $"<{(Animated ? "a" : "")}:{Name}:{Id}>";
    }
}
