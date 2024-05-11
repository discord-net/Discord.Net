using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based channel in a guild that can send and receive messages.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestTextChannel : RestGuildChannel, IRestMessageChannel, ITextChannel
    {
        #region RestTextChannel
        /// <inheritdoc />
        public string Topic { get; private set; }
        /// <inheritdoc />
        public virtual int SlowModeInterval { get; private set; }
        /// <inheritdoc />
        public ulong? CategoryId { get; private set; }
        /// <inheritdoc />
        public string Mention => MentionUtils.MentionChannel(Id);
        /// <inheritdoc />
        public bool IsNsfw { get; private set; }
        /// <inheritdoc />
        public ThreadArchiveDuration DefaultArchiveDuration { get; private set; }
        /// <inheritdoc />
        public int DefaultSlowModeInterval { get; private set; }

        internal RestTextChannel(BaseDiscordClient discord, IGuild guild, ulong id, ulong guildId)
            : base(discord, guild, id, guildId)
        {
        }
        internal new static RestTextChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestTextChannel(discord, guild, model.Id, guild?.Id ?? model.GuildId.Value);
            entity.Update(model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(Model model)
        {
            base.Update(model);
            CategoryId = model.CategoryId;
            Topic = model.Topic.GetValueOrDefault();
            if (model.SlowMode.IsSpecified)
                SlowModeInterval = model.SlowMode.Value;
            IsNsfw = model.Nsfw.GetValueOrDefault();

            if (model.AutoArchiveDuration.IsSpecified)
                DefaultArchiveDuration = model.AutoArchiveDuration.Value;
            else
                DefaultArchiveDuration = ThreadArchiveDuration.OneDay;

            DefaultSlowModeInterval = model.ThreadRateLimitPerUser.GetValueOrDefault(0);
            // basic value at channel creation. Shouldn't be called since guild text channels always have this property
        }

        /// <inheritdoc />
        public virtual async Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        /// <summary>
        ///     Gets a user in this channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="InvalidOperationException">
        /// Resolving permissions requires the parent guild to be downloaded.
        /// </exception>
        /// <returns>
        ///     A task representing the asynchronous get operation. The task result contains a guild user object that
        ///     represents the user; <see langword="null" /> if none is found.
        /// </returns>
        public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetUserAsync(this, Guild, Discord, id, options);

        /// <summary>
        ///     Gets a collection of users that are able to view the channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="InvalidOperationException">
        /// Resolving permissions requires the parent guild to be downloaded.
        /// </exception>
        /// <returns>
        ///     A paged collection containing a collection of guild users that can access this channel. Flattening the
        ///     paginated response into a collection of users with
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the users.
        /// </returns>
        public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions options = null)
            => ChannelHelper.GetUsersAsync(this, Guild, Discord, null, null, options);

        /// <inheritdoc />
        public virtual Task<RestMessage> GetMessageAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetMessageAsync(this, Discord, id, options);
        /// <inheritdoc />
        public virtual IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, null, Direction.Before, limit, options);
        /// <inheritdoc />
        public virtual IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit, options);
        /// <inheritdoc />
        public virtual IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit, options);
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
        /// <exception cref="ArgumentException">
        /// <paramref name="filePath" /> is a zero-length string, contains only white space, or contains one or more
        /// invalid characters as defined by <see cref="System.IO.Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filePath" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length. For example, on
        /// Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260
        /// characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="filePath" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="filePath" /> was not found.
        /// </exception>
        /// <exception cref="NotSupportedException"><paramref name="filePath" /> is in an invalid format.</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
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
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, isSpoiler, embeds, flags, poll);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendFileAsync(FileAttachment attachment, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, attachment, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/>, <see cref="MessageFlags.SuppressNotification"/> and <see cref="MessageFlags.None"/>.</exception>
        public virtual Task<RestUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFilesAsync(this, Discord, attachments, text, isTTS, embed, allowedMentions, messageReference, components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        public virtual Task DeleteMessageAsync(ulong messageId, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, messageId, Discord, options);
        /// <inheritdoc />
        public virtual Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, message.Id, Discord, options);

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
        public virtual Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        /// <inheritdoc />
        public virtual IDisposable EnterTypingState(RequestOptions options = null)
            => ChannelHelper.EnterTypingState(this, Discord, options);

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
        public virtual async Task<RestThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread,
            ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
        {
            var model = await ThreadHelper.CreateThreadAsync(Discord, this, name, type, autoArchiveDuration, message, invitable, slowmode, options);
            return RestThreadChannel.Create(Discord, Guild, model);
        }

        /// <summary>
        ///     Gets the parent (category) channel of this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the category channel
        ///     representing the parent of this channel; <see langword="null" /> if none is set.
        /// </returns>
        public virtual Task<ICategoryChannel> GetCategoryAsync(RequestOptions options = null)
            => ChannelHelper.GetCategoryAsync(this, Discord, options);
        /// <inheritdoc />
        public Task SyncPermissionsAsync(RequestOptions options = null)
            => ChannelHelper.SyncPermissionsAsync(this, Discord, options);

        /// <inheritdoc cref="ITextChannel.GetActiveThreadsAsync(RequestOptions)"/>
        public virtual Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
            => ThreadHelper.GetActiveThreadsAsync(Guild, Id, Discord, options);
        #endregion

        #region Invites
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);
        public virtual async Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge = 86400, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToApplicationAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, applicationId, options);
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteToApplicationAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, (ulong)application, options);
        public virtual Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => throw new NotImplementedException();
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";
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

        #endregion

        #region ITextChannel

        async Task<IThreadChannel> ITextChannel.CreateThreadAsync(string name, ThreadType type, ThreadArchiveDuration autoArchiveDuration, IMessage message, bool? invitable, int? slowmode, RequestOptions options)
            => await CreateThreadAsync(name, type, autoArchiveDuration, message, invitable, slowmode, options);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IThreadChannel>> ITextChannel.GetActiveThreadsAsync(RequestOptions options)
            => await GetActiveThreadsAsync(options);
        #endregion

        #region IMessageChannel
        /// <inheritdoc />
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetMessageAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(limit, options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }

        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(fromMessageId, dir, limit, options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetMessagesAsync(fromMessage, dir, limit, options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
        }
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

        #region IGuildChannel
        /// <inheritdoc />
        async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetUserAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            return mode == CacheMode.AllowDownload
                ? GetUsersAsync(options)
                : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
        }
        #endregion

        #region IChannel
        /// <inheritdoc />
        async Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetUserAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return GetUsersAsync(options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
        }
        #endregion

        #region INestedChannel
        /// <inheritdoc />
        async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        {
            if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
                return (await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false)) as ICategoryChannel;
            return null;
        }
        #endregion
    }
}
