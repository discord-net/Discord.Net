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
    ///     Represents a WebSocket-based direct-message channel.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketDMChannel : SocketChannel, IDMChannel, ISocketPrivateChannel, ISocketMessageChannel
    {
        #region SocketDMChannel
        /// <summary>
        ///     Gets the recipient of the channel.
        /// </summary>
        public SocketUser Recipient { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> CachedMessages => ImmutableArray.Create<SocketMessage>();

        /// <summary>
        ///     Gets a collection that is the current logged-in user and the recipient.
        /// </summary>
        public new IReadOnlyCollection<SocketUser> Users => ImmutableArray.Create(Discord.CurrentUser, Recipient);

        internal SocketDMChannel(DiscordSocketClient discord, ulong id, SocketUser recipient)
            : base(discord, id)
        {
            Recipient = recipient;
        }
        internal static SocketDMChannel Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketDMChannel(discord, model.Id, discord.GetOrCreateTemporaryUser(state, model.Recipients.Value[0]));
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            Recipient.Update(state, model.Recipients.Value[0]);
        }
        internal static SocketDMChannel Create(DiscordSocketClient discord, ClientState state, ulong channelId, API.User recipient)
        {
            var entity = new SocketDMChannel(discord, channelId, discord.GetOrCreateTemporaryUser(state, recipient));
            entity.Update(state, recipient);
            return entity;
        }
        internal void Update(ClientState state, API.User recipient)
        {
            Recipient.Update(state, recipient);
        }

        /// <inheritdoc />
        public Task CloseAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);
        #endregion

        #region Messages
        /// <inheritdoc />
        public SocketMessage GetCachedMessage(ulong id)
            => null;
        /// <summary>
        ///     Gets the message associated with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">TThe ID of the message.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The message gotten from either the cache or the download, or <see langword="null" /> if none is found.
        /// </returns>
        public async Task<IMessage> GetMessageAsync(ulong id, RequestOptions options = null)
        {
            return await ChannelHelper.GetMessageAsync(this, Discord, id, options).ConfigureAwait(false);
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
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, null, Direction.Before, limit, options);
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
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit, options);
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
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit, options);
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
            => ImmutableArray.Create<SocketMessage>();
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ImmutableArray.Create<SocketMessage>();
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ImmutableArray.Create<SocketMessage>();
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord, options);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null,
            RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null,
            RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, isSpoiler, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, isSpoiler, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(FileAttachment attachment, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, attachment, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFilesAsync(this, Discord, attachments, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        public Task DeleteMessageAsync(ulong messageId, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, messageId, Discord, options);
        /// <inheritdoc />
        public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
            => ChannelHelper.DeleteMessageAsync(this, message.Id, Discord, options);

        /// <inheritdoc />
        public async Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions options = null)
            => await ChannelHelper.ModifyMessageAsync(this, messageId, func, Discord, options).ConfigureAwait(false);

        /// <inheritdoc />
        public Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        /// <inheritdoc />
        public IDisposable EnterTypingState(RequestOptions options = null)
            => ChannelHelper.EnterTypingState(this, Discord, options);

        internal void AddMessage(SocketMessage msg)
        {
        }
        internal SocketMessage RemoveMessage(ulong id)
        {
            return null;
        }
        #endregion

        #region Users
        /// <summary>
        ///     Gets a user in this channel from the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <returns>
        ///     A <see cref="SocketUser"/> object that is a recipient of this channel; otherwise <see langword="null" />.
        /// </returns>
        public new SocketUser GetUser(ulong id)
        {
            if (id == Recipient.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return Discord.CurrentUser;
            else
                return null;
        }

        /// <summary>
        ///     Returns the recipient user.
        /// </summary>
        public override string ToString() => $"@{Recipient}";
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
        internal new SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;
        #endregion

        #region SocketChannel
        /// <inheritdoc />
        internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;
        /// <inheritdoc />
        internal override SocketUser GetUserInternal(ulong id) => GetUser(id);
        #endregion

        #region IDMChannel
        /// <inheritdoc />
        IUser IDMChannel.Recipient => Recipient;
        #endregion

        #region ISocketPrivateChannel
        /// <inheritdoc />
        IReadOnlyCollection<SocketUser> ISocketPrivateChannel.Recipients => ImmutableArray.Create(Recipient);
        #endregion

        #region IPrivateChannel
        /// <inheritdoc />
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);
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
            => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(limit, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(fromMessageId, dir, limit, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(fromMessage.Id, dir, limit, options);
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

        #region IChannel
        /// <inheritdoc />
        string IChannel.Name => $"@{Recipient}";

        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
        #endregion
    }
}
