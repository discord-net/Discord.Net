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

        internal RestTextChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }
        internal new static RestTextChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestTextChannel(discord, guild, model.Id);
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
        ///     represents the user; <c>null</c> if none is found.
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
        public Task<RestMessage> GetMessageAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetMessageAsync(this, Discord, id, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, null, Direction.Before, limit, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit, options);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<RestUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent component = null, ISticker[] stickers = null)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS, embed, allowedMentions, messageReference, component, stickers, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentException">
        /// <paramref name="filePath" /> is a zero-length string, contains only white space, or contains one or more
        /// invalid characters as defined by <see cref="System.IO.Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filePath" /> is <c>null</c>.
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
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent component = null, ISticker[] stickers = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, embed, allowedMentions, messageReference, component, stickers, options, isSpoiler);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent component = null, ISticker[] stickers = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, embed, allowedMentions, messageReference, component, stickers, options, isSpoiler);

        /// <inheritdoc />
        public Task DeleteMessageAsync(ulong messageId, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, messageId, Discord, options);
        /// <inheritdoc />
        public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, message.Id, Discord, options);

        /// <inheritdoc />
        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages.Select(x => x.Id), options);
        /// <inheritdoc />
        public Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messageIds, options);

        /// <inheritdoc />
        public async Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions options = null)
            => await ChannelHelper.ModifyMessageAsync(this, messageId, func, Discord, options).ConfigureAwait(false);

        /// <inheritdoc />
        public Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        /// <inheritdoc />
        public IDisposable EnterTypingState(RequestOptions options = null)
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
        ///     with the identifier; <c>null</c> if the webhook is not found.
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
        ///     Gets the parent (category) channel of this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the category channel
        ///     representing the parent of this channel; <c>null</c> if none is set.
        /// </returns>
        public virtual Task<ICategoryChannel> GetCategoryAsync(RequestOptions options = null)
            => ChannelHelper.GetCategoryAsync(this, Discord, options);
        /// <inheritdoc />
        public Task SyncPermissionsAsync(RequestOptions options = null)
            => ChannelHelper.SyncPermissionsAsync(this, Discord, options);
        #endregion

        #region Invites
        /// <inheritdoc />
        public virtual async Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);
        public virtual Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => throw new NotImplementedException();
        public virtual Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge, int? maxUses = default(int?), bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => throw new NotImplementedException();
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";

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
        ///     <para>
        ///         <b>Note: </b> Options <see cref="ThreadArchiveDuration.OneWeek"/> and <see cref="ThreadArchiveDuration.ThreeDays"/>
        ///         are only available for guilds that are boosted. You can check in the <see cref="IGuild.Features"/> to see if the 
        ///         guild has the <b>THREE_DAY_THREAD_ARCHIVE</b> and <b>SEVEN_DAY_THREAD_ARCHIVE</b>.
        ///     </para>
        /// </param>
        /// <param name="message">The message which to start the thread from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous create operation. The task result contains a <see cref="IThreadChannel"/>
        /// </returns>
        public async Task<RestThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread,
            ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, RequestOptions options = null)
        {
            var model = await ThreadHelper.CreateThreadAsync(Discord, this, name, type, autoArchiveDuration, message, options);
            return RestThreadChannel.Create(Discord, this.Guild, model);
        }
        #endregion

        #region ITextChannel
        /// <inheritdoc />
        async Task<IWebhook> ITextChannel.CreateWebhookAsync(string name, Stream avatar, RequestOptions options)
            => await CreateWebhookAsync(name, avatar, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IWebhook> ITextChannel.GetWebhookAsync(ulong id, RequestOptions options)
            => await GetWebhookAsync(id, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IWebhook>> ITextChannel.GetWebhooksAsync(RequestOptions options)
            => await GetWebhooksAsync(options).ConfigureAwait(false);

        async Task<IThreadChannel> ITextChannel.CreateThreadAsync(string name, ThreadType type, ThreadArchiveDuration autoArchiveDuration, IMessage message, RequestOptions options)
            => await CreateThreadAsync(name, type, autoArchiveDuration, message, options);
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
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS, Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent component, ISticker[] stickers)
            => await SendFileAsync(filePath, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, component, stickers).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS, Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent component, ISticker[] stickers)
            => await SendFileAsync(stream, filename, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, component, stickers).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent component, ISticker[] stickers)
            => await SendMessageAsync(text, isTTS, embed, options, allowedMentions, messageReference, component, stickers).ConfigureAwait(false);
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
            if (mode == CacheMode.AllowDownload)
                return GetUsersAsync(options);
            else
                return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
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

        #region ITextChannel
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
