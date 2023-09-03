using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

/// <summary>
///     Represents a channel that can contain invites.
/// </summary>
public interface IInvitableChannel : IGuildChannel
{
    /// <summary>
    ///     Creates a new invite to this channel.
    /// </summary>
    /// <param name="maxAge">The time (in seconds) until the invite expires. Set to <see langword="null" /> to never expire.</param>
    /// <param name="maxUses">The max amount of times this invite may be used. Set to <see langword="null" /> to have unlimited uses.</param>
    /// <param name="isTemporary">If <see langword="true" />, the user accepting this invite will be kicked from the guild after closing their client.</param>
    /// <param name="isUnique">If <see langword="true" />, don't try to reuse a similar invite (useful for creating many unique one time use invites).</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous invite creation operation. The task result contains an invite
    ///     metadata object containing information for the created invite.
    /// </returns>
    Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Creates a new invite to this channel.
    /// </summary>
    /// <param name="applicationId">The id of the embedded application to open for this invite.</param>
    /// <param name="maxAge">The time (in seconds) until the invite expires. Set to <see langword="null" /> to never expire.</param>
    /// <param name="maxUses">The max amount of times this invite may be used. Set to <see langword="null" /> to have unlimited uses.</param>
    /// <param name="isTemporary">If <see langword="true" />, the user accepting this invite will be kicked from the guild after closing their client.</param>
    /// <param name="isUnique">If <see langword="true" />, don't try to reuse a similar invite (useful for creating many unique one time use invites).</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous invite creation operation. The task result contains an invite
    ///     metadata object containing information for the created invite.
    /// </returns>
    Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Creates a new invite to this channel.
    /// </summary>
    /// <example>
    ///     <para>The following example creates a new invite to this channel; the invite lasts for 12 hours and can only
    ///     be used 3 times throughout its lifespan.</para>
    ///     <code language="cs">
    ///     await guildChannel.CreateInviteAsync(maxAge: 43200, maxUses: 3);
    ///     </code>
    /// </example>
    /// <param name="user">The id of the user whose stream to display for this invite.</param>
    /// <param name="maxAge">The time (in seconds) until the invite expires. Set to <see langword="null" /> to never expire.</param>
    /// <param name="maxUses">The max amount of times this invite may be used. Set to <see langword="null" /> to have unlimited uses.</param>
    /// <param name="isTemporary">If <see langword="true" />, the user accepting this invite will be kicked from the guild after closing their client.</param>
    /// <param name="isUnique">If <see langword="true" />, don't try to reuse a similar invite (useful for creating many unique one time use invites).</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous invite creation operation. The task result contains an invite
    ///     metadata object containing information for the created invite.
    /// </returns>
    Task<IInviteMetadata> CreateInviteToStreamAsync(ulong userId, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Gets a collection of all invites to this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of invite metadata that are created for this channel.
    /// </returns>
    Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions? options = null, CancellationToken token = default);
}
