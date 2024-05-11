using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based direct-message channel.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestDMChannel : RestChannel, IDMChannel, IRestPrivateChannel, IRestMessageChannel
    {
        #region RestDMChannel
        /// <summary>
        ///     Gets the current logged-in user.
        /// </summary>
        public RestUser CurrentUser { get; }

        /// <summary>
        ///     Gets the recipient of the channel.
        /// </summary>
        public RestUser Recipient { get; }

        /// <summary>
        ///     Gets a collection that is the current logged-in user and the recipient.
        /// </summary>
        public IReadOnlyCollection<RestUser> Users => Recipient is null
            ? ImmutableArray<RestUser>.Empty
            : ImmutableArray.Create(CurrentUser, Recipient);

        internal RestDMChannel(BaseDiscordClient discord, ulong id, ulong? recipientId)
            : base(discord, id)
        {
            Recipient = recipientId.HasValue ? new RestUser(Discord, recipientId.Value) : null;
            CurrentUser = new RestUser(Discord, discord.CurrentUser.Id);
        }
        internal new static RestDMChannel Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestDMChannel(discord, model.Id, model.Recipients.GetValueOrDefault(null)?[0].Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            if(model.Recipients.IsSpecified)
                Recipient?.Update(model.Recipients.Value[0]);
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetChannelAsync(Id, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public Task CloseAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);


        /// <summary>
        ///     Gets a user in this channel from the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <returns>
        ///     A <see cref="RestUser"/> object that is a recipient of this channel; otherwise <see langword="null" />.
        /// </returns>
        public RestUser GetUser(ulong id)
        {
            if (id == Recipient?.Id)
                return Recipient;
            else if (id == Discord.CurrentUser.Id)
                return CurrentUser;
            else
                return null;
        }

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
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null,
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
        /// <exception cref="ArgumentException">The only valid <see cref="MessageFlags"/> are <see cref="MessageFlags.SuppressEmbeds"/> and <see cref="MessageFlags.None"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null,
            RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, embed, allowedMentions, messageReference,
            components, stickers, options, isSpoiler, embeds, flags, poll);
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, embed, allowedMentions,
                messageReference, components, stickers, options, isSpoiler, embeds, flags, poll);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<RestUserMessage> SendFileAsync(FileAttachment attachment, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFileAsync(this, Discord, attachment, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, embeds, flags, poll);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<RestUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, PollProperties poll = null)
            => ChannelHelper.SendFilesAsync(this, Discord, attachments, text, isTTS, embed, allowedMentions, messageReference,
                components, stickers, options, embeds, flags, poll);

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

        /// <summary>
        ///     Gets a string that represents the Username#Discriminator of the recipient.
        /// </summary>
        /// <returns>
        ///     A string that resolves to the Recipient of this channel.
        /// </returns>
        public override string ToString() => $"@{Recipient}";
        private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
        #endregion

        #region IDMChannel
        /// <inheritdoc />
        IUser IDMChannel.Recipient => Recipient;
        #endregion

        #region IRestPrivateChannel
        /// <inheritdoc />
        IReadOnlyCollection<RestUser> IRestPrivateChannel.Recipients => ImmutableArray.Create(Recipient);
        #endregion

        #region IPrivateChannel
        /// <inheritdoc />
        IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);
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
