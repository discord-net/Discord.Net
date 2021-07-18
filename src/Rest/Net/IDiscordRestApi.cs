using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Models;

namespace Discord.Net.Rest
{
    internal interface IDiscordRestApi
    {
        #region Audit Log

        /// <summary>
        /// Gets an <see cref="AuditLog"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/audit-log#get-guild-audit-log"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an <see cref="AuditLog"/>.
        /// </returns>
        Task<AuditLog> GetGuildAuditLogAsync(Snowflake guildId, GetGuildAuditLogParams args, CancellationToken cancellationToken = default);

        #endregion

        #region Channel

        /// <summary>
        /// Gets a <see cref="Channel"/> by their identifier.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#get-channel"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="Channel"/> if it exists;
        /// otherwise, <see langword="null"/>.
        /// </returns>
        Task<Channel?> GetChannelAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a <see cref="GroupChannel"/>'s settings.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#modify-channel"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="GroupChannel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="GroupChannel"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<GroupChannel> ModifyChannelAsync(Snowflake channelId, ModifyGroupChannelParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a <see cref="GuildChannel"/>'s settings.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#modify-channel"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="GuildChannel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="GuildChannel"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<GuildChannel> ModifyChannelAsync(Snowflake channelId, ModifyGuildChannelParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a <see cref="ThreadChannel"/>'s settings.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#modify-channel"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="ThreadChannel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="ThreadChannel"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<ThreadChannel> ModifyChannelAsync(Snowflake channelId, ModifyThreadChannelParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Channel"/>, or closes a <see cref="PrivateChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#deleteclose-channel"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the deleted <see cref="Channel"/>.
        /// </returns>
        Task<Channel> DeleteChannelAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all specified <see cref="Message"/>s from a <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#get-channel-messages"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an array of <see cref="Message"/>s from the <see cref="Channel"/>.
        /// </returns>
        Task<IEnumerable<Message>> GetChannelMessagesAsync(Snowflake channelId, GetChannelMessagesParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific <see cref="Message"/> from a <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#get-channel-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the specified <see cref="Message"/> or <see langword="null"/>.
        /// </returns>
        Task<Message?> GetChannelMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a <see cref="Message"/> to a <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#create-message"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the <see cref="Message"/> created.
        /// </returns>
        Task<Message> CreateMessageAsync(Snowflake channelId, CreateMessageParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Crossposts a <see cref="Message"/> in a News <see cref="Channel"/> to following channels.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#crosspost-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the crossposted <see cref="Message"/>.
        /// </returns>
        Task<Message> CrosspostMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="Reaction"/> for a <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#create-reaction"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task CreateReactionAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Reaction"/> the current user has made for the <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#delete-own-reaction"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteOwnReactionAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes another <see cref="User"/>'s <see cref="Reaction"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#delete-user-reaction"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteUserReactionAsync(Snowflake channelId, Snowflake messageId, Snowflake userId, Emoji emoji, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="User"/>s that reacted with this <see cref="Emoji"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#get-reactions"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an array of <see cref="User"/>s that reacted with
        /// the provided <see cref="Emoji"/>.
        /// </returns>
        Task<IEnumerable<User>> GetReactionsAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, GetReactionsParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all <see cref="Reaction"/>s on a <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#delete-all-reactions"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteAllReactionsAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all the <see cref="Reaction"/>s for a given <see cref="Emoji"/>
        /// on a <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#delete-all-reactions-for-emoji"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteAllReactionsforEmojiAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits a previously sent <see cref="Message"/>. 
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#edit-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="Message"/>.
        /// </returns>
        Task<Message> EditMessageAsync(Snowflake channelId, Snowflake messageId, EditMessageParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#delete-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple <see cref="Message"/>s in a single request.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#bulk-delete-messages"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task BulkDeleteMessagesAsync(Snowflake channelId, BulkDeleteMessagesParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits the <see cref="Channel"/> permission <see cref="Overwrite"/>s for a <see cref="User"/> or <see cref="Role"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#edit-channel-permissions"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="overwriteId">
        /// The <see cref="Overwrite"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task EditChannelPermissionsAsync(Snowflake channelId, Snowflake overwriteId, EditChannelPermissionsParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="InviteWithMetadata"/>s for a <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#get-channel-invites"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an array of <see cref="InviteWithMetadata"/>s.
        /// </returns>
        Task<IEnumerable<InviteWithMetadata>> GetChannelInvitesAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="Invite"/> for the <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#create-channel-invite"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="Invite"/>.
        /// </returns>
        Task<Invite> CreateChannelInviteAsync(Snowflake channelId, CreateChannelInviteParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Channel"/> permission <see cref="Overwrite"/> for a <see cref="User"/> or <see cref="Role"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#delete-channel-permission"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="overwriteId">
        /// The <see cref="Overwrite"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteChannelPermissionAsync(Snowflake channelId, Snowflake overwriteId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Follow a News <see cref="Channel"/> to send <see cref="Message"/>s to a target <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#follow-news-channel"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the <see cref="FollowedChannel"/>.
        /// </returns>
        Task<FollowedChannel> FollowNewsChannelAsync(Snowflake channelId, FollowNewsChannelParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Posts a typing indicator for the specified <see cref="Channel"/>.
        /// </summary>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task TriggerTypingIndicatorAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all pinned <see cref="Message"/>s in the <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#get-pinned-messages"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an array of all pinned <see cref="Message"/>s.
        /// </returns>
        Task<IEnumerable<Message>> GetPinnedMessagesAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Pins a <see cref="Message"/> in a <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#pin-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task PinMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unpins a <see cref="Message"/> in a <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#unpin-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task UnpinMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a recipient to a <see cref="GroupChannel"/> using their access token.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#group-dm-add-recipient"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task GroupDMAddRecipientAsync(Snowflake channelId, Snowflake userId, GroupDMAddRecipientParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a recipient from a <see cref="GroupChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#group-dm-remove-recipient"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task GroupDMRemoveRecipientAsync(Snowflake channelId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="ThreadChannel"/> from an existing <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#start-thread-with-message"/>
        /// </remarks>
        /// <param name="messageId">
        /// The <see cref="Message"/> identifier.
        /// </param>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="ThreadChannel"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<ThreadChannel> StartThreadAsync(Snowflake channelId, Snowflake messageId, StartThreadWithMessageParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new private <see cref="ThreadChannel"/> that is not connected to an existing <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#start-thread-without-message"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="ThreadChannel"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<ThreadChannel> StartThreadAsync(Snowflake channelId, StartThreadWithoutMessageParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the current user to a <see cref="ThreadChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#join-thread"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="ThreadChannel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task JoinThreadAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds another <see cref="GuildMember"/> to a <see cref="ThreadChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#add-thread-member"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="ThreadChannel"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="GuildMember"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task AddThreadMemberAsync(Snowflake channelId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the current user from a <see cref="ThreadChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#leave-thread"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="ThreadChannel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task LeaveThreadAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes another <see cref="GuildMember"/> from a <see cref="ThreadChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#remove-thread-member"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="ThreadChannel"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="GuildMember"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task RemoveThreadMemberAsync(Snowflake channelId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="ThreadMember"/>s that are part of the <see cref="ThreadChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#list-thread-members"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="ThreadChannel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an array of <see cref="ThreadMember"/>s that are part of the
        /// specified <see cref="ThreadChannel"/>.
        /// </returns>
        Task<IEnumerable<ThreadMember>> ListThreadMembersAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active <see cref="ThreadChannel"/>s in the <see cref="Channel"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#list-active-threads"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="ThreadList"/>.
        /// </returns>
        Task<ThreadList> ListActiveThreadsAsync(Snowflake channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets archived <see cref="ThreadChannel"/>s in the <see cref="Channel"/> that are public.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#list-public-archived-threads"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="ThreadList"/>.
        /// </returns>
        Task<ThreadList> ListPublicArchivedThreadsAsync(Snowflake channelId, ListPublicArchivedThreadsParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets archived <see cref="ThreadChannel"/>s in the <see cref="Channel"/> that are private.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#list-private-archived-threads"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="ThreadList"/>.
        /// </returns>
        Task<ThreadList> ListPrivateArchivedThreadsAsync(Snowflake channelId, ListPrivateArchivedThreadsParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets archived <see cref="ThreadChannel"/>s in the <see cref="Channel"/> that are
        /// private and the current user has joined.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/channel#list-joined-private-archived-threads"/>
        /// </remarks>
        /// <param name="channelId">
        /// The <see cref="Channel"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="ThreadList"/>.
        /// </returns>
        Task<ThreadList> ListJoinedPrivateArchivedThreadsAsync(Snowflake channelId, ListJoinedPrivateArchivedThreadsParams args, CancellationToken cancellationToken = default);

        #endregion Channel

        #region Emoji

        /// <summary>
        /// Gets all <see cref="Emoji"/>s for the given <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/emoji#list-guild-emojis"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an array of <see cref="Emoji"/>s.
        /// </returns>
        Task<IEnumerable<Emoji>> ListGuildEmojisAsync(Snowflake guildId, Emoji emoji, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an <see cref="Emoji"/> for the given <see cref="Guild"/> and <see cref="Emoji"/> IDs.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/emoji#get-guild-emoji"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains an <see cref="Emoji"/>.
        /// </returns>
        Task<Emoji> GetGuildEmojiAsync(Snowflake guildId, Emoji emoji, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="Emoji"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/emoji#create-guild-emoji"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="Emoji"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<Emoji> CreateGuildEmojiAsync(Snowflake guildId, Emoji emoji, CreateGuildEmojiParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modify the given <see cref="Emoji"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/emoji#modify-guild-emoji"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="Emoji"/>.
        /// </returns>
        Task<Emoji> ModifyGuildEmojiAsync(Snowflake guildId, Emoji emoji, ModifyGuildEmojiParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete the given <see cref="Emoji"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/emoji#delete-guild-emoji"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="emoji">
        /// An <see cref="Emoji"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteGuildEmojiAsync(Snowflake guildId, Emoji emoji, CancellationToken cancellationToken = default);

        #endregion Emoji

        #region Guild

        /// <summary>
        /// Creates a new <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#create-guild"/>
        /// </remarks>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="Guild"/>.
        /// </returns>
        Task<Guild> CreateGuildAsync(CreateGuildParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="Guild"/> for the given id.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the requested <see cref="Guild"/> if it exists; or <see langword="null"/>.
        /// </returns>
        Task<Guild> GetGuildAsync(Snowflake guildId, GetGuildParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="Guild"/> preview for the given id.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-preview"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the requested <see cref="Guild"/> preview if it exists and is
        /// viewable; or <see langword="null"/>.
        /// </returns>
        Task<Guild?> GetGuildPreviewAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies a <see cref="Guild"/>'s settings.
        /// </summary>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="Guild"/>.
        /// </returns>
        Task<Guild> ModifyGuildAsync(Snowflake guildId, ModifyGuildParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Guild"/> permanently.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#delete-guild"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteGuildAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="GuildChannel"/>s. It does not include <see cref="ThreadChannel"/>s.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-channels"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="GuildChannel"/>s.
        /// </returns>
        Task<IEnumerable<GuildChannel>> GetGuildChannelsAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="GuildChannel"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#create-guild-channel"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="GuildChannel"/>.
        /// </returns>
        Task<GuildChannel> CreateGuildChannelAsync(Snowflake guildId, CreateGuildChannelParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the positions of a set of <see cref="GuildChannel"/>s for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-guild-channel-positions"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task ModifyGuildChannelPositionsAsync(Snowflake guildId, ModifyGuildChannelPositionsParams[] args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specified <see cref="GuildMember"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-member"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the requested <see cref="GuildMember"/>; or <see langword="null"/> if not found.
        /// </returns>
        Task<GuildMember?> GetGuildMemberAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="GuildMember"/> that are members of the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#list-guild-members"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the returned collection of <see cref="GuildMember"/>s.
        /// </returns>
        Task<IEnumerable<GuildMember>> ListGuildMembersAsync(Snowflake guildId, ListGuildMembersParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="GuildMember"/>s whose username or nickname starts with a provided string.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#search-guild-members"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the returned collection of <see cref="GuildMember"/>s.
        /// </returns>
        Task<IEnumerable<GuildMember>> SearchGuildMembersAsync(Snowflake guildId, SearchGuildMembersParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a <see cref="User"/> to the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#add-guild-member"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the <see cref="GuildMember"/> related to the provided <see cref="User"/>;
        /// or <see langword="null"/> if they are already a part of the <see cref="Guild"/>.
        /// </returns>
        /// <exception cref="DiscordRestException">
        /// Thrown when invalid parameters are supplied in <paramref name="args"/>.
        /// </exception>
        Task<GuildMember?> AddGuildMemberAsync(Snowflake guildId, Snowflake userId, AddGuildMemberParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies attributes of a <see cref="GuildMember"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-guild-member"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="GuildMember"/>.
        /// </returns>
        Task<GuildMember> ModifyGuildMemberAsync(Snowflake guildId, Snowflake userId, ModifyGuildMemberParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the nickname of the current user in a <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-current-user-nick"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        ///  <returns>
        /// A task that contains the updated nickname.
        /// </returns>
        Task<string> ModifyCurrentUserNickAsync(Snowflake guildId, ModifyCurrentUserNickParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a <see cref="Role"/> to a <see cref="GuildMember"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#add-guild-member-role"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="roleId">
        /// The <see cref="Role"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task AddGuildMemberRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a <see cref="Role"/> from a <see cref="GuildMember"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#remove-guild-member-role"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="roleId">
        /// The <see cref="Role"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task RemoveGuildMemberRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a <see cref="GuildMember"/> from a <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#remove-guild-member"/>
        /// </remarks>
        /// <param name="guildId">
        /// The The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task RemoveGuildMemberAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="Ban"/>s for the users banned from this <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-bans"/>
        /// </remarks>
        /// <param name="guildId">
        /// The The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="Ban"/>s.
        /// </returns>
        Task<IEnumerable<Ban>> GetGuildBansAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="Ban"/> for the given <see cref="User"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-ban"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the <see cref="Ban"/>; or <see langword="null"/> if not found.
        /// </returns>
        Task<Ban?> GetGuildBanAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a guild <see cref="Ban"/>, and optionally delete previous messages sent by the banned <see cref="User"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#create-guild-ban"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task CreateGuildBanAsync(Snowflake guildId, Snowflake userId, CreateGuildBanParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the <see cref="Ban"/> for a <see cref="User"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#remove-guild-ban"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task RemoveGuildBanAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="Role"/>s for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-roles"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="Role"/>s.
        /// </returns>
        Task<IEnumerable<Role>> GetGuildRolesAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="Role"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#create-guild-role"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the created <see cref="Role"/>.
        /// </returns>
        Task<Role> CreateGuildRoleAsync(Snowflake guildId, CreateGuildRoleParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the positions of a set of <see cref="Role"/>s for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-guild-role-positions"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="Role"/>s.
        /// </returns>
        Task<IEnumerable<Role>> ModifyGuildRolePositionsAsync(Snowflake guildId, ModifyGuildRolePositionsParams[] args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies a <see cref="Guild"/> <see cref="Role"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-guild-role"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="roleId">
        /// The <see cref="Role"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="Role"/>.
        /// </returns>
        Task<Role> ModifyGuildRoleAsync(Snowflake guildId, Snowflake roleId, ModifyGuildRoleParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Guild"/> <see cref="Role"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#delete-guild-role"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="roleId">
        /// The <see cref="Role"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteGuildRoleAsync(Snowflake guildId, Snowflake roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="Prune"/> with the number of members that would be removed in a prune operation.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-prune-count"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the a <see cref="Prune"/> with the number of members that would be removed in a prune operation.
        /// </returns>
        Task<Prune> GetGuildPruneCountAsync(Snowflake guildId, GetGuildPruneCountParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a prune operation.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#begin-guild-prune"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the a <see cref="Prune"/> with the number of members that were removed in the prune operation.
        /// </returns>
        Task<Prune> BeginGuildPruneAsync(Snowflake guildId, BeginGuildPruneParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="VoiceRegion"/>s for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-voice-regions"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="VoiceRegion"/>s.
        /// </returns>
        Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="InviteWithMetadata"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-invites"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="InviteWithMetadata"/>s.
        /// </returns>
        Task<IEnumerable<InviteWithMetadata>> GetGuildInvitesAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="Integration"/>s for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-integrations"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a collection of <see cref="Integration"/>s.
        /// </returns>
        Task<IEnumerable<Integration>> GetGuildIntegrationsAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the attached <see cref="Integration"/> for the <see cref="Guild"/>.
        /// Deletes any associated <see cref="Webhook"/>s and kicks the associated bot if there is one.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#delete-guild-integration"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task DeleteGuildIntegrationAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="GuildWidget"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-widget-settings"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="GuildWidget"/>.
        /// </returns>
        Task<GuildWidget> GetGuildWidgetSettingsAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies a <see cref="GuildWidget"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-guild-widget"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="GuildWidget"/>.
        /// </returns>
        Task<GuildWidget> ModifyGuildWidgetAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        // TODO: Create a Widget object or remove this?
        /// <summary>
        /// Gets the widget for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-widget"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the widget for the <see cref="Guild"/>.
        /// </returns>
        Task<object> GetGuildWidgetAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a partial <see cref="Invite"/> for <see cref="Guild"/>s with a vanity url enabled.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-vanity-url"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a partial <see cref="Invite"/> if the <see cref="Guild"/> has
        /// this feature enabled; or <see langword="null"/> otherwise.
        /// </returns>
        Task<Invite?> GetGuildVanityURLAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="WelcomeScreen"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-welcome-screen"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains a <see cref="WelcomeScreen"/>.
        /// </returns>
        Task<WelcomeScreen> GetGuildWelcomeScreenAsync(Snowflake guildId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the <see cref="WelcomeScreen"/> for the <see cref="Guild"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-guild-welcome-screen"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that contains the updated <see cref="WelcomeScreen"/>.
        /// </returns>
        Task<WelcomeScreen> ModifyGuildWelcomeScreenAsync(Snowflake guildId, ModifyGuildWelcomeScreenParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the current user's <see cref="VoiceState"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-current-user-voice-state"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task ModifyCurrentUserVoiceStateAsync(Snowflake guildId, ModifyCurrentUserVoiceStateParams args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates another user's <see cref="VoiceState"/>.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/guild#modify-user-voice-state"/>
        /// </remarks>
        /// <param name="guildId">
        /// The <see cref="Guild"/> identifier.
        /// </param>
        /// <param name="userId">
        /// The <see cref="User"/> identifier.
        /// </param>
        /// <param name="args">
        /// Parameters to include in the request.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token for the request.
        /// </param>
        /// <returns>
        /// A task that represents this asynchronous operation.
        /// </returns>
        Task ModifyUserVoiceStateAsync(Snowflake guildId, Snowflake userId, ModifyUserVoiceStateParams args, CancellationToken cancellationToken = default);

        #endregion Guild
    }
}
