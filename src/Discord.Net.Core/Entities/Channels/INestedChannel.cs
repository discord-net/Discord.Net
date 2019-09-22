using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a type of guild channel that can be nested within a category.
    /// </summary>
    public interface INestedChannel : IGuildChannel
    {
        /// <summary>
        ///     Gets the parent (category) ID of this channel in the guild's channel list.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the parent of this channel; 
        ///     <c>null</c> if none is set.
        /// </returns>
        ulong? CategoryId { get; }
        /// <summary>
        ///     Gets the parent (category) channel of this channel.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the category channel
        ///     representing the parent of this channel; <c>null</c> if none is set.
        /// </returns>
        Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Syncs the permissions of this nested channel with its parent's.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for syncing channel permissions with its parent's.
        /// </returns>
        Task SyncPermissionsAsync(RequestOptions options = null);

        /// <summary>
        ///     Creates a new invite to this channel.
        /// </summary>
        /// <example>
        ///     The following example creates a new invite to this channel; the invite lasts for 12 hours and can only
        ///     be used 3 times throughout its lifespan.
        ///     <code language="cs">
        ///     await guildChannel.CreateInviteAsync(maxAge: 43200, maxUses: 3);
        ///     </code>
        /// </example>
        /// <param name="maxAge">The time (in seconds) until the invite expires. Set to <c>null</c> to never expire.</param>
        /// <param name="maxUses">The max amount of times this invite may be used. Set to <c>null</c> to have unlimited uses.</param>
        /// <param name="isTemporary">If <c>true</c>, the user accepting this invite will be kicked from the guild after closing their client.</param>
        /// <param name="isUnique">If <c>true</c>, don't try to reuse a similar invite (useful for creating many unique one time use invites).</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous invite creation operation. The task result contains an invite
        ///     metadata object containing information for the created invite.
        /// </returns>
        Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of all invites to this channel.
        /// </summary>B
        /// <example>
        ///     The following example gets all of the invites that have been created in this channel and selects the
        ///     most used invite.
        ///     <code language="cs">
        ///     var invites = await channel.GetInvitesAsync();
        ///     if (invites.Count == 0) return;
        ///     var invite = invites.OrderByDescending(x => x.Uses).FirstOrDefault();
        ///     </code>
        /// </example>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of invite metadata that are created for this channel.
        /// </returns>
        Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null);
    }
}
