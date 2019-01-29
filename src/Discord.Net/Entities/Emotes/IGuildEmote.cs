using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IGuildEmote : IEmote, ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     Gets whether this emoji is managed by an integration.
        /// </summary>
        /// <returns>
        ///     A boolean that determines whether or not this emote is managed by a Twitch integration.
        /// </returns>
        bool IsManaged { get; }
        /// <summary>
        ///     Gets whether this emoji must be wrapped in colons.
        /// </summary>
        /// <returns>
        ///     A boolean that determines whether or not this emote requires the use of colons in chat to be used.
        /// </returns>
        bool RequireColons { get; }
        /// <summary>
        ///     Gets the roles that are allowed to use this emoji.
        /// </summary>
        /// <returns>
        ///     A read-only list containing snowflake identifiers for roles that are allowed to use this emoji.
        /// </returns>
        IReadOnlyList<IRole> Roles { get; }
        /// <summary>
        ///     Gets the user ID associated with the creation of this emoji.
        /// </summary>
        /// <returns>
        ///     An <see cref="ulong"/> snowflake identifier representing the user who created this emoji; 
        ///     <c>null</c> if unknown.
        /// </returns>
        ulong? CreatorId { get; }
        /// <summary>
        ///     Gets the guild this emote sourced from.
        /// </summary>
        IGuild Guild { get; }

        Task ModifyAsync(); // TODO
    }
}
