using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based channel in a guild that can send and receive messages.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketMessageChannel
    {
        #region SocketTextChannel
        private readonly MessageCache _messages;

        /// <inheritdoc />
        public string Topic { get; private set; }
        /// <inheritdoc />
        public virtual int SlowModeInterval { get; private set; }
        /// <inheritdoc />
        public ulong? CategoryId { get; private set; }
        /// <inheritdoc />
        public int DefaultSlowModeInterval { get; private set; }
        /// <summary>
        ///     Gets the parent (category) of this channel in the guild's channel list.
        /// </summary>
        /// <returns>
        ///     An <see cref="ICategoryChannel"/> representing the parent of this channel; <see langword="null" /> if none is set.
        /// </returns>
        public ICategoryChannel Category
            => CategoryId.HasValue ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel : null;
        /// <inheritdoc />
        public virtual Task SyncPermissionsAsync(RequestOptions options = null)
            => ChannelHelper.SyncPermissionsAsync(this, Discord, options);

        private bool _nsfw;
        /// <inheritdoc />
        public bool IsNsfw => _nsfw;
        /// <inheritdoc />
        public ThreadArchiveDuration DefaultArchiveDuration { get; private set; }
        /// <inheritdoc />
        public string Mention => MentionUtils.MentionChannel(Id);
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? ImmutableArray.Create<SocketMessage>();
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuildUser> Users
            => Guild.Users.Where(x => Permissions.GetValue(
                Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
                ChannelPermission.ViewChannel)).ToImmutableArray();

        /// <summary>
        ///     Gets a collection of threads within this text channel.
        /// </summary>
        public IReadOnlyCollection<SocketThreadChannel> Threads
            => Guild.ThreadChannels.Where(x => x.ParentChannel.Id == Id).ToImmutableArray();

        internal SocketTextChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
            if (Discord?.MessageCacheSize > 0)
                _messages = new MessageCache(Discord);
        }
        internal new static SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketTextChannel(guild?.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
            CategoryId = model.CategoryId;
            Topic = model.Topic.GetValueOrDefault();
            SlowModeInterval = model.SlowMode.GetValueOrDefault(); // some guilds haven't been patched to include this yet?
            _nsfw = model.Nsfw.GetValueOrDefault();
            if (model.AutoArchiveDuration.IsSpecified)
                DefaultArchiveDuration = model.AutoArchiveDuration.Value;
            else
                DefaultArchiveDuration = ThreadArchiveDuration.OneDay;

            DefaultSlowModeInterval = model.ThreadRateLimitPerUser.GetValueOrDefault(0);
            // basic value at channel creation. Shouldn't be called since guild text channels always have this property
        }

        /// <inheritdoc />
        public virtual Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);

        /// <summary>
        ///     Creates a thread within this <see cref="ITextChannel"/>.
        /// </summary>
        /// <remarks>
        ///     When <paramref name="message"/> is <see langword="null"/> the thread type will be based off of the
        ///     channel its created in. When called on a <see cref="ITextChannel"/>, it creates a <see cref="ThreadType.PublicThread"/>.
        ///     When called on a <see cref="INewsChannel"/>, it creates a <see cref="ThreadType.NewsThread"/>. The id of the created
        ///     thread will be the same as the id of the message, and as such a message can only have a
        ///     single thread created from it.
        /// </remarks>
        /// <param name="name">The name of the thread.</param>
        /// <param name="type">
        ///     The type of the thread.
        ///     <para>
        ///         <b>Note: </b>This parameter is not used if the <paramref name="message"/> parameter is not specified.
        ///     </para>
        /// </param>
        /// <param name="autoArchiveDuration">
        ///     The duration on which this thread archives after.
        /// </param>
        /// <param name="message">The message which to start the thread from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous create operation. The task result contains a <see cref="IThreadChannel"/>
        /// </returns>
        public virtual async Task<SocketThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread,
            ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
        {
            var model = await ThreadHelper.CreateThreadAsync(Discord, this, name, type, autoArchiveDuration, message, invitable, slowmode, options);

            var thread = (SocketThreadChannel)Guild.AddOrUpdateChannel(Discord.State, model);

            if (Discord.AlwaysDownloadUsers && Discord.HasGatewayIntent(GatewayIntents.GuildMembers))
                await thread.DownloadUsersAsync();

            return thread;
        }

        /// <inheritdoc cref="ITextChannel.GetActiveThreadsAsync(RequestOptions)"/>
        public virtual Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
            => ThreadHelper.GetActiveThreadsAsync(Guild, Id, Discord, options);

        #endregion

        #region Messages
        /// <inheritdoc />
        public virtual SocketMessage GetCachedMessage(ulong id)
            => _messages?.Get(id);
        /// <summary>
        ///     Gets a message from this message channel.
        /// </summary>
        /// <remarks>
        ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessageAsync"/>.
        ///     Please visit its documentation for more details on this method.
        /// </remarks>
        /// <param name="id">The snowflake identifier of the message.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
        ///     the retrieved message; <see langword="null" /> if no message is found with the specified identifier.
        /// </returns>
        public virtual async Task<IMessage> GetMessageAsync(ulong id, RequestOptions options = null)
        {
            IMessage msg = _messages?.Get(id);
            if (msg == null)
                msg = await ChannelHelper.GetMessageAsync(this, Discord, id, options).ConfigureAwait(false);
            return msg;
        }

        /// <summary>
        ///     Gets the last N messages from this message channel.
        /// </summary>
        /// <remarks>
        ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(int, CacheMode, RequestOptions)"/>.
        ///     Please visit its documentation for more details on this method.
        /// </remarks>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages.
        /// </returns>
        public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, CacheMode.AllowDownload, options);
        /// <summary>
        ///     Gets a collection of messages in this channel.
        /// </summary>
        /// <remarks>
        ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(ulong, Direction, int, CacheMode, RequestOptions)"/>.
        ///     Please visit its documentation for more details on this method.
        /// </remarks>
        /// <param name="fromMessageId">The ID of the starting message to get the messages from.</param>
        /// <param name="dir">The direction of the messages to be gotten from.</param>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages.
        /// </returns>
        public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, CacheMode.AllowDownload, options);
        /// <summary>
        ///     Gets a collection of messages in this channel.
        /// </summary>
        /// <remarks>
        ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(IMessage, Direction, int, CacheMode, RequestOptions)"/>.
        ///     Please visit its documentation for more details on this method.
        /// </remarks>
        /// <param name="fromMessage">The starting message to get the messages from.</param>
        /// <param name="dir">The direction of the messages to be gotten from.</param>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages.
        /// </returns>
        public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, CacheMode.AllowDownload, options);
        /// <inheritdoc />
        public virtual IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, null, Direction.Before, limit);
        /// <inheritdoc />
        public virtual IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessageId, dir, limit);
        /// <inheritdoc />
        public virtual IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessage.Id, dir, limit);
        /// <inheritdoc />
        public virtual Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null,
            RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null,
            RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, isSpoiler, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, isSpoiler, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendFileAsync(FileAttachment attachment, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, attachment, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFilesAsync(this, Discord, attachments, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        public virtual Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages.Select(x => x.Id), options);
        /// <inheritdoc />
        public virtual Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messageIds, options);

        /// <inheritdoc />
        public virtual async Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions options = null)
            => await ChannelHelper.ModifyMessageAsync(this, messageId, func, Discord, options).ConfigureAwait(false);

        /// <inheritdoc />
        public virtual Task DeleteMessageAsync(ulong messageId, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, messageId, Discord, options);
        /// <inheritdoc />
        public virtual Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, message.Id, Discord, options);

        /// <inheritdoc />
        public virtual Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        /// <inheritdoc />
        public virtual IDisposable EnterTypingState(RequestOptions options = null)
            => ChannelHelper.EnterTypingState(this, Discord, options);

        internal void AddMessage(SocketMessage msg)
            => _messages?.Add(msg);
        internal SocketMessage RemoveMessage(ulong id)
            => _messages?.Remove(id);
        #endregion

        #region Users
        /// <inheritdoc />
        public override SocketGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user != null)
            {
                var guildPerms = Permissions.ResolveGuild(Guild, user);
                var channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
                if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel))
                    return user;
            }
            return null;
        }
        #endregion

        #region Webhooks
        /// <summary>
        ///     Creates a webhook in this text channel.
        /// </summary>
        /// <param name="name">The name of the webhook.</param>
        /// <param name="avatar">The avatar of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     webhook.
        /// </returns>
        public virtual Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
            => ChannelHelper.CreateWebhookAsync(this, Discord, name, avatar, options);
        /// <summary>
        ///     Gets a webhook available in this text channel.
        /// </summary>
        /// <param name="id">The identifier of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a webhook associated
        ///     with the identifier; <see langword="null" /> if the webhook is not found.
        /// </returns>
        public virtual Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetWebhookAsync(this, Discord, id, options);
        /// <summary>
        ///     Gets the webhooks available in this text channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of webhooks that is available in this channel.
        /// </returns>
        public virtual Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => ChannelHelper.GetWebhooksAsync(this, Discord, options);
        #endregion

        #region Invites
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToApplicationAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, applicationId, options).ConfigureAwait(false);
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToApplicationAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, (ulong)application, options);
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToStreamAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, user, options).ConfigureAwait(false);
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";
        internal new SocketTextChannel Clone() => MemberwiseClone() as SocketTextChannel;
        #endregion

        #region IIntegrationChannel

        /// <inheritdoc />
        async Task<IWebhook> IIntegrationChannel.CreateWebhookAsync(string name, Stream avatar, RequestOptions options)
            => await CreateWebhookAsync(name, avatar, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IWebhook> IIntegrationChannel.GetWebhookAsync(ulong id, RequestOptions options)
            => await GetWebhookAsync(id, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IWebhook>> IIntegrationChannel.GetWebhooksAsync(RequestOptions options)
            => await GetWebhooksAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        #endregion

        #region ITextChannel
        async Task<IThreadChannel> ITextChannel.CreateThreadAsync(string name, ThreadType type, ThreadArchiveDuration autoArchiveDuration, IMessage message, bool? invitable, int? slowmode, RequestOptions options)
            => await CreateThreadAsync(name, type, autoArchiveDuration, message, invitable, slowmode, options);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IThreadChannel>> ITextChannel.GetActiveThreadsAsync(RequestOptions options)
            => await GetActiveThreadsAsync(options);
        #endregion

        #region IGuildChannel
        /// <inheritdoc />
        async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            var user = GetUser(id);
            if (user is not null || mode == CacheMode.CacheOnly)
                return user;

            return await ChannelHelper.GetUserAsync(this, Guild, Discord, id, options).ConfigureAwait(false);
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            return mode == CacheMode.AllowDownload
                ? ChannelHelper.GetUsersAsync(this, Guild, Discord, null, null, options)
                : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        }

        #endregion

        #region IMessageChannel
        /// <inheritdoc />
        Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessageAsync(id, options);
            else
                return Task.FromResult((IMessage)GetCachedMessage(id));
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, mode, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, mode, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, mode, options);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync(RequestOptions options)
            => await GetPinnedMessagesAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS, Embed embed,
            RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageReference messageReference,
            MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, PollProperties poll)
            => await SendFileAsync(filePath, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference,
            components, stickers, embeds, flags, poll).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS,
            Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageReference messageReference,
            MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, PollProperties poll)
            => await SendFileAsync(stream, filename, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference,
                components, stickers, embeds, flags, poll).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFileAsync(FileAttachment attachment, string text, bool isTTS,
            Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageReference messageReference,
            MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, PollProperties poll)
            => await SendFileAsync(attachment, text, isTTS, embed, options, allowedMentions, messageReference, components,
                stickers, embeds, flags, poll).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFilesAsync(IEnumerable<FileAttachment> attachments, string text,
            bool isTTS, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageReference messageReference,
            MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, PollProperties poll)
           => await SendFilesAsync(attachments, text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags, poll).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS, Embed embed, RequestOptions options,
            AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent components,
            ISticker[] stickers, Embed[] embeds, MessageFlags flags, PollProperties poll)
            => await SendMessageAsync(text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags, poll).ConfigureAwait(false);

        #endregion

        #region  INestedChannel
        /// <inheritdoc />
        Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult(Category);
        #endregion
    }
}
