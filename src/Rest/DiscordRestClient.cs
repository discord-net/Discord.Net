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
        public Task<Channel?> GetChannelAsync(Snowflake channelId, CancellationToken cancellationToken = default)
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
        public Task<IEnumerable<Message>> GetChannelMessagesAsync(Snowflake channelId, GetChannelMessagesParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetChannelMessagesAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message?> GetChannelMessageAsync(Snowflake channelId, Snowflake messageId, CancellationToken cancellationToken = default)
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
        public Task<IEnumerable<User>> GetReactionsAsync(Snowflake channelId, Snowflake messageId, Emoji emoji, GetReactionsParams args, CancellationToken cancellationToken = default)
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
        public Task<IEnumerable<InviteWithMetadata>> GetChannelInvitesAsync(Snowflake channelId, CancellationToken cancellationToken = default)
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
        public Task<IEnumerable<Message>> GetPinnedMessagesAsync(Snowflake channelId, CancellationToken cancellationToken = default)
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
        public Task<IEnumerable<ThreadMember>> ListThreadMembersAsync(Snowflake channelId, CancellationToken cancellationToken = default)
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

        #region Emoji

        /// <inheritdoc/>
        public Task<IEnumerable<Emoji>> ListGuildEmojisAsync(Snowflake guildId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.ListGuildEmojisAsync(guildId, emoji, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Emoji> GetGuildEmojiAsync(Snowflake guildId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.GetGuildEmojiAsync(guildId, emoji, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Emoji> CreateGuildEmojiAsync(Snowflake guildId, Emoji emoji, CreateGuildEmojiParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(emoji, nameof(emoji));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildEmojiAsync(guildId, emoji, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Emoji> ModifyGuildEmojiAsync(Snowflake guildId, Emoji emoji, ModifyGuildEmojiParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(emoji, nameof(emoji));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyGuildEmojiAsync(guildId, emoji, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteGuildEmojiAsync(Snowflake guildId, Emoji emoji, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(emoji, nameof(emoji));
            return _api.DeleteGuildEmojiAsync(guildId, emoji, cancellationToken);
        }

        #endregion Emoji

        #region Guild

        /// <inheritdoc/>
        public Task<Guild> CreateGuildAsync(CreateGuildParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildAsync(args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Guild> GetGuildAsync(Snowflake guildId, GetGuildParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetGuildAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Guild?> GetGuildPreviewAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildPreviewAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Guild> ModifyGuildAsync(Snowflake guildId, ModifyGuildParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyGuildAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteGuildAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.DeleteGuildAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GuildChannel>> GetGuildChannelsAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildChannelsAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildChannel> CreateGuildChannelAsync(Snowflake guildId, CreateGuildChannelParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildChannelAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ModifyGuildChannelPositionsAsync(Snowflake guildId, ModifyGuildChannelPositionsParams[] args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            foreach (var value in args)
            {
                Preconditions.NotNull(value, nameof(args));
                value.Validate();
            }
            return _api.ModifyGuildChannelPositionsAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildMember?> GetGuildMemberAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.GetGuildMemberAsync(guildId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GuildMember>> ListGuildMembersAsync(Snowflake guildId, ListGuildMembersParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ListGuildMembersAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GuildMember>> SearchGuildMembersAsync(Snowflake guildId, SearchGuildMembersParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.SearchGuildMembersAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildMember?> AddGuildMemberAsync(Snowflake guildId, Snowflake userId, AddGuildMemberParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.AddGuildMemberAsync(guildId, userId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildMember> ModifyGuildMemberAsync(Snowflake guildId, Snowflake userId, ModifyGuildMemberParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyGuildMemberAsync(guildId, userId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<string> ModifyCurrentUserNickAsync(Snowflake guildId, ModifyCurrentUserNickParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyCurrentUserNickAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddGuildMemberRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotZero(roleId, nameof(roleId));
            return _api.AddGuildMemberRoleAsync(guildId, userId, roleId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveGuildMemberRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotZero(roleId, nameof(roleId));
            return _api.RemoveGuildMemberRoleAsync(guildId, userId, roleId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveGuildMemberAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.RemoveGuildMemberAsync(guildId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Ban>> GetGuildBansAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildBansAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Ban?> GetGuildBanAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.GetGuildBanAsync(guildId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task CreateGuildBanAsync(Snowflake guildId, Snowflake userId, CreateGuildBanParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildBanAsync(guildId, userId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveGuildBanAsync(Snowflake guildId, Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            return _api.RemoveGuildBanAsync(guildId, userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Role>> GetGuildRolesAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildRolesAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Role> CreateGuildRoleAsync(Snowflake guildId, CreateGuildRoleParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildRoleAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Role>> ModifyGuildRolePositionsAsync(Snowflake guildId, ModifyGuildRolePositionsParams[] args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            foreach (var value in args)
            {
                Preconditions.NotNull(value, nameof(args));
                value.Validate();
            }
            return _api.ModifyGuildRolePositionsAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Role> ModifyGuildRoleAsync(Snowflake guildId, Snowflake roleId, ModifyGuildRoleParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(roleId, nameof(roleId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyGuildRoleAsync(guildId, roleId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteGuildRoleAsync(Snowflake guildId, Snowflake roleId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(roleId, nameof(roleId));
            return _api.DeleteGuildRoleAsync(guildId, roleId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Prune> GetGuildPruneCountAsync(Snowflake guildId, GetGuildPruneCountParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetGuildPruneCountAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Prune> BeginGuildPruneAsync(Snowflake guildId, BeginGuildPruneParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.BeginGuildPruneAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildVoiceRegionsAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<InviteWithMetadata>> GetGuildInvitesAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildInvitesAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Integration>> GetGuildIntegrationsAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildIntegrationsAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteGuildIntegrationAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.DeleteGuildIntegrationAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildWidget> GetGuildWidgetSettingsAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildWidgetSettingsAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildWidget> ModifyGuildWidgetAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.ModifyGuildWidgetAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<object> GetGuildWidgetAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildWidgetAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Invite?> GetGuildVanityURLAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildVanityURLAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WelcomeScreen> GetGuildWelcomeScreenAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildWelcomeScreenAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WelcomeScreen> ModifyGuildWelcomeScreenAsync(Snowflake guildId, ModifyGuildWelcomeScreenParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyGuildWelcomeScreenAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ModifyCurrentUserVoiceStateAsync(Snowflake guildId, ModifyCurrentUserVoiceStateParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyCurrentUserVoiceStateAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ModifyUserVoiceStateAsync(Snowflake guildId, Snowflake userId, ModifyUserVoiceStateParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotZero(userId, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyUserVoiceStateAsync(guildId, userId, args, cancellationToken);
        }

        #endregion Guild

        #region Guild Template

        /// <inheritdoc/>
        public Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNullOrEmpty(templateCode, nameof(templateCode));
            return _api.GetGuildTemplateAsync(templateCode, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Guild> CreateGuildfromGuildTemplateAsync(string templateCode, CreateGuildfromGuildTemplateParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNullOrEmpty(templateCode, nameof(templateCode));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildfromGuildTemplateAsync(templateCode, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildTemplatesAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildTemplate> CreateGuildTemplateAsync(Snowflake guildId, CreateGuildTemplateParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateGuildTemplateAsync(guildId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildTemplate> SyncGuildTemplateAsync(Snowflake guildId, string templateCode, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNullOrEmpty(templateCode, nameof(templateCode));
            return _api.SyncGuildTemplateAsync(guildId, templateCode, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildTemplate> ModifyGuildTemplateAsync(Snowflake guildId, string templateCode, ModifyGuildTemplateParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNullOrEmpty(templateCode, nameof(templateCode));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyGuildTemplateAsync(guildId, templateCode, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<GuildTemplate> DeleteGuildTemplateAsync(Snowflake guildId, string templateCode, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            Preconditions.NotNullOrEmpty(templateCode, nameof(templateCode));
            return _api.DeleteGuildTemplateAsync(guildId, templateCode, cancellationToken);
        }

        #endregion Guild Template

        #region Invite

        /// <inheritdoc/>
        public Task<Invite> GetInviteAsync(string inviteCode, GetInviteParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetInviteAsync(inviteCode, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Invite> DeleteInviteAsync(string inviteCode, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));
            return _api.DeleteInviteAsync(inviteCode, cancellationToken);
        }

        #endregion Invite

        #region Stage Instance

        /// <inheritdoc/>
        public Task<StageInstance> CreateStageInstanceAsync(CreateStageInstanceParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateStageInstanceAsync(args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<StageInstance> GetStageInstanceAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.GetStageInstanceAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<StageInstance> ModifyStageInstanceAsync(Snowflake channelId, ModifyStageInstanceParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyStageInstanceAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteStageInstanceAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.DeleteStageInstanceAsync(channelId, cancellationToken);
        }

        #endregion Stage Instance

        #region User

        /// <inheritdoc/>
        public Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            return _api.GetCurrentUserAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<User> GetUserAsync(Snowflake userId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(userId, nameof(userId));
            return _api.GetUserAsync(userId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<User> ModifyCurrentUserAsync(ModifyCurrentUserParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyCurrentUserAsync(args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Guild>> GetCurrentUserGuildsAsync(GetCurrentUserGuildsParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.GetCurrentUserGuildsAsync(args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task LeaveGuildAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.LeaveGuildAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<DMChannel> CreateDMAsync(CreateDMParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateDMAsync(args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Connection>> GetUserConnectionsAsync(CancellationToken cancellationToken = default)
        {
            return _api.GetUserConnectionsAsync(cancellationToken);
        }

        #endregion User

        #region Voice

        /// <inheritdoc/>
        public Task<IEnumerable<VoiceRegion>> ListVoiceRegionsAsync(CancellationToken cancellationToken = default)
        {
            return _api.ListVoiceRegionsAsync(cancellationToken);
        }

        #endregion Voice

        #region Webhook

        /// <inheritdoc/>
        public Task<Webhook> CreateWebhookAsync(Snowflake channelId, CreateWebhookParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.CreateWebhookAsync(channelId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Webhook>> GetChannelWebhooksAsync(Snowflake channelId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(channelId, nameof(channelId));
            return _api.GetChannelWebhooksAsync(channelId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Webhook>> GetGuildWebhooksAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(guildId, nameof(guildId));
            return _api.GetGuildWebhooksAsync(guildId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Webhook> GetWebhookAsync(Snowflake webhookId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            return _api.GetWebhookAsync(webhookId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Webhook> GetWebhookAsync(Snowflake webhookId, string webhookToken, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(webhookToken, nameof(webhookToken));
            return _api.GetWebhookAsync(webhookId, webhookToken, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Webhook> ModifyWebhookAsync(Snowflake webhookId, ModifyWebhookParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyWebhookAsync(webhookId, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Webhook> ModifyWebhookAsync(Snowflake webhookId, string webhookToken, ModifyWebhookWithTokenParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ModifyWebhookAsync(webhookId, webhookToken, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteWebhookAsync(Snowflake webhookId, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            return _api.DeleteWebhookAsync(webhookId, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteWebhookAsync(Snowflake webhookId, string webhookToken, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(webhookToken, nameof(webhookToken));
            return _api.DeleteWebhookAsync(webhookId, webhookToken, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> ExecuteWebhookAsync(Snowflake webhookId, string webhookToken, ExecuteWebhookParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(webhookToken, nameof(webhookToken));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.ExecuteWebhookAsync(webhookId, webhookToken, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> GetWebhookMessageAsync(Snowflake messageId, Snowflake webhookId, string webhookToken, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(webhookToken, nameof(webhookToken));
            return _api.GetWebhookMessageAsync(messageId, webhookId, webhookToken, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Message> EditWebhookMessageAsync(Snowflake messageId, Snowflake webhookId, string webhookToken, EditWebhookMessageParams args, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(webhookToken, nameof(webhookToken));
            Preconditions.NotNull(args, nameof(args));
            args.Validate();
            return _api.EditWebhookMessageAsync(messageId, webhookId, webhookToken, args, cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteWebhookMessageAsync(Snowflake messageId, Snowflake webhookId, string webhookToken, CancellationToken cancellationToken = default)
        {
            Preconditions.NotZero(messageId, nameof(messageId));
            Preconditions.NotZero(webhookId, nameof(webhookId));
            Preconditions.NotNull(webhookToken, nameof(webhookToken));
            return _api.DeleteWebhookMessageAsync(messageId, webhookId, webhookToken, cancellationToken);
        }

        #endregion Webhook
    }
}
