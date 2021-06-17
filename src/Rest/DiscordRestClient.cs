using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Models;
using Discord.Net.Rest;

namespace Discord.Net
{
    /// <summary>
    /// TBD
    /// </summary>
    public class DiscordRestClient : IDiscordRestApi
    {
        private readonly IDiscordRestApi _api;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="api"></param>
        internal DiscordRestClient(IDiscordRestApi api)
        {
            _api = api;
        }

        #region Audit Log

        /// <inheritdoc/>
        public Task<AuditLog> GetGuildAuditLogAsync(Snowflake guildId, GetGuildAuditLogParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetGuildAuditLogAsync(guildId, args, cancellationToken);
        }

        #endregion

        #region Channel

        /// <inheritdoc/>
        public Task<Channel> GetChannelAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.GetChannelAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GroupChannel> ModifyChannelAsync(Snowflake channelId, ModifyGroupChannelParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyChannelAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildChannel> ModifyChannelAsync(Snowflake channelId, ModifyGuildChannelParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyChannelAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadChannel> ModifyChannelAsync(Snowflake channelId, ModifyThreadChannelParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyChannelAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Channel> DeleteChannelAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.DeleteChannelAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message[]> GetChannelMessagesAsync(Snowflake channelId, GetChannelMessagesParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetChannelMessagesAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> GetChannelMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            return _api.GetChannelMessageAsync(channelId, messageId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> CreateMessageAsync(Snowflake channelId, CreateMessageParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateMessageAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> CrosspostMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            return _api.CrosspostMessageAsync(channelId, messageId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task CreateReactionAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.CreateReactionAsync(channelId, messageId, emoji, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteOwnReactionAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.DeleteOwnReactionAsync(channelId, messageId, emoji, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteUserReactionAsync(Snowflake channelId, Snowflake messageId, Snowflake userId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.DeleteUserReactionAsync(channelId, messageId, userId, emoji, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<User[]> GetReactionsAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, GetReactionsParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotNull(emoji, nameof(emoji));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetReactionsAsync(channelId, messageId, emoji, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteAllReactionsAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.DeleteAllReactionsAsync(messageId, channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteAllReactionsforEmojiAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.DeleteAllReactionsforEmojiAsync(channelId, messageId, emoji, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> EditMessageAsync(Snowflake channelId, Snowflake messageId, EditMessageParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.EditMessageAsync(channelId, messageId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            return _api.DeleteMessageAsync(channelId, messageId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task BulkDeleteMessagesAsync(Snowflake channelId, BulkDeleteMessagesParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.BulkDeleteMessagesAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task EditChannelPermissionsAsync(Snowflake channelId, Snowflake overwriteId, EditChannelPermissionsParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(overwriteId, nameof(overwriteId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.EditChannelPermissionsAsync(channelId, overwriteId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<InviteWithMetadata[]> GetChannelInvitesAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.GetChannelInvitesAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Invite> CreateChannelInviteAsync(Snowflake channelId, CreateChannelInviteParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateChannelInviteAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteChannelPermissionAsync(Snowflake channelId, Snowflake overwriteId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(overwriteId, nameof(overwriteId));
            return _api.DeleteChannelPermissionAsync(channelId, overwriteId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FollowedChannel> FollowNewsChannelAsync(Snowflake channelId, FollowNewsChannelParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.FollowNewsChannelAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task TriggerTypingIndicatorAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.TriggerTypingIndicatorAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message[]> GetPinnedMessagesAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.GetPinnedMessagesAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task PinMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            return _api.PinMessageAsync(channelId, messageId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task UnpinMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            return _api.UnpinMessageAsync(channelId, messageId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task GroupDMAddRecipientAsync(Snowflake channelId, Snowflake userId, GroupDMAddRecipientParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GroupDMAddRecipientAsync(channelId, userId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task GroupDMRemoveRecipientAsync(Snowflake channelId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.GroupDMRemoveRecipientAsync(channelId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadChannel> StartThreadAsync(Snowflake channelId, Snowflake messageId, StartThreadWithMessageParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.StartThreadAsync(channelId, messageId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadChannel> StartThreadAsync(Snowflake channelId, StartThreadWithoutMessageParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.StartThreadAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task JoinThreadAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.JoinThreadAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddThreadMemberAsync(Snowflake channelId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.AddThreadMemberAsync(channelId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task LeaveThreadAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.LeaveThreadAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveThreadMemberAsync(Snowflake channelId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.RemoveThreadMemberAsync(channelId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadMember[]> ListThreadMembersAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.ListThreadMembersAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadList> ListActiveThreadsAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.ListActiveThreadsAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadList> ListPublicArchivedThreadsAsync(Snowflake channelId, ListPublicArchivedThreadsParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ListPublicArchivedThreadsAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadList> ListPrivateArchivedThreadsAsync(Snowflake channelId, ListPrivateArchivedThreadsParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ListPrivateArchivedThreadsAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<ThreadList> ListJoinedPrivateArchivedThreadsAsync(Snowflake channelId, ListJoinedPrivateArchivedThreadsParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ListJoinedPrivateArchivedThreadsAsync(channelId, args, cancellationToken);
        }

        #endregion Channel
    }
}
