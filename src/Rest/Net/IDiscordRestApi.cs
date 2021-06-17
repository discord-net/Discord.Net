using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task<Channel> GetChannelAsync(Snowflake channelId, CancellationToken cancellationToken = default);

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
        /// Gets an array of <see cref="Message"/>s from a <see cref="Channel"/>.
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
        Task<Message[]> GetChannelMessagesAsync(Snowflake channelId, GetChannelMessagesParams args, CancellationToken cancellationToken = default);

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
        Task<Message> GetChannelMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default);

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
        /// Gets an array of <see cref="User"/>s that reacted with this <see cref="Emoji"/>.
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
        Task<User[]> GetReactionsAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, GetReactionsParams args, CancellationToken cancellationToken = default);

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
        /// Gets an array of <see cref="Invite"/>s for a <see cref="Channel"/>.
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
        Task<InviteWithMetadata[]> GetChannelInvitesAsync(Snowflake channelId, CancellationToken cancellationToken = default);

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
        Task<Message[]> GetPinnedMessagesAsync(Snowflake channelId, CancellationToken cancellationToken = default);

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
        /// Gets an array of <see cref="ThreadMember"/>s that are part of the <see cref="ThreadChannel"/>.
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
        Task<ThreadMember[]> ListThreadMembersAsync(Snowflake channelId, CancellationToken cancellationToken = default);

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


    }
}
