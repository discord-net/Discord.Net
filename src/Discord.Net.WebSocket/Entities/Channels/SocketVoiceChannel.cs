using Discord.Audio;
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
    ///     Represents a WebSocket-based voice channel in a guild.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel, ISocketAudioChannel
    {
        #region SocketVoiceChannel
        /// <summary>
        ///     Gets whether or not the guild has Text-In-Voice enabled and the voice channel is a TiV channel.
        /// </summary>
        public virtual bool IsTextInVoice
            => Guild.Features.HasTextInVoice;

        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int? UserLimit { get; private set; }
        /// <inheritdoc />
        public string RTCRegion { get; private set; }

        /// <summary>
        ///     Gets a collection of users that are currently connected to this voice channel.
        /// </summary>
        /// <returns>
        ///     A read-only collection of users that are currently connected to this voice channel.
        /// </returns>
        public IReadOnlyCollection<SocketGuildUser> ConnectedUsers
            => Guild.Users.Where(x => x.VoiceChannel?.Id == Id).ToImmutableArray();

        internal SocketVoiceChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
        }
        internal new static SocketVoiceChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketVoiceChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
            Bitrate = model.Bitrate.Value;
            UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;
            RTCRegion = model.RTCRegion.GetValueOrDefault(null);
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);

        /// <inheritdoc />
        public async Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false)
        {
            return await Guild.ConnectAudioAsync(Id, selfDeaf, selfMute, external).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
            => await Guild.DisconnectAudioAsync();

        /// <inheritdoc />
        public async Task ModifyAsync(Action<AudioChannelProperties> func, RequestOptions options = null)
        {
            await Guild.ModifyAudioAsync(Id, func, options).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override SocketGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user?.VoiceChannel?.Id == Id)
                return user;
            return null;
        }

        /// <inheritdoc/> <exception cref="InvalidOperationException">Cannot create threads in voice channels.</exception>
        public override Task<SocketThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new InvalidOperationException("Voice channels cannot contain threads.");

        /// <inheritdoc/> <exception cref="InvalidOperationException">Cannot modify text channel properties for voice channels.</exception>
        public override Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot modify text channel properties for voice channels.");

        #endregion

        #region TextOverrides

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<IMessage> GetMessageAsync(ulong id, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetMessageAsync(id, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.DeleteMessageAsync(message, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task DeleteMessageAsync(ulong messageId, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.DeleteMessageAsync(messageId, options); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.DeleteMessagesAsync(messages, options); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.DeleteMessagesAsync(messageIds, options); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IDisposable EnterTypingState(RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.EnterTypingState(options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override SocketMessage GetCachedMessage(ulong id)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetCachedMessage(id); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = 100)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetCachedMessages(fromMessage, dir, limit);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = 100)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetCachedMessages(limit);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = 100)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetCachedMessages(fromMessageId, dir, limit);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetMessagesAsync(fromMessage, dir, limit, options); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetMessagesAsync(limit, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetMessagesAsync(fromMessageId, dir, limit, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetPinnedMessagesAsync(options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetWebhookAsync(id, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetWebhooksAsync(options); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.CreateWebhookAsync(name, avatar, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.ModifyMessageAsync(messageId, func, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestUserMessage> SendFileAsync(FileAttachment attachment, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.SendFileAsync(attachment, text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags); 
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.SendFileAsync(stream, filename, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, components, stickers, embeds, flags);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.SendFileAsync(filePath, text, isTTS, embed, options, isSpoiler, allowedMentions, messageReference, components, stickers, embeds, flags);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.SendFilesAsync(attachments, text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.SendMessageAsync(text, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds, flags);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task TriggerTypingAsync(RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.TriggerTypingAsync(options);
        }

        #endregion

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
        internal new SocketVoiceChannel Clone() => MemberwiseClone() as SocketVoiceChannel;

        #region IGuildChannel
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        #endregion

        #region INestedChannel
        /// <inheritdoc />
        Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult(Category);
        #endregion
    }
}
