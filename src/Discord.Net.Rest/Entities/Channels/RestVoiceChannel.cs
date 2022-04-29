using Discord.Audio;
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
    ///     Represents a REST-based voice channel in a guild.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestVoiceChannel : RestTextChannel, IVoiceChannel, IRestAudioChannel
    {
        #region RestVoiceChannel
        /// <summary>
        ///     Gets whether or not the guild has Text-In-Voice enabled and the voice channel is a TiV channel.
        /// </summary>
        public virtual bool IsTextInVoice
            => Guild.Features.HasTextInVoice;
        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int? UserLimit { get; private set; }
        /// <inheritdoc/>
        public string RTCRegion { get; private set; }

        internal RestVoiceChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }
        internal new static RestVoiceChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestVoiceChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        /// <inheritdoc />
        internal override void Update(Model model)
        {
            base.Update(model);

            if(model.Bitrate.IsSpecified)
                Bitrate = model.Bitrate.Value;

            if(model.UserLimit.IsSpecified)
                UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;

            RTCRegion = model.RTCRegion.GetValueOrDefault(null);
        }

        /// <inheritdoc />
        public async Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot modify text channel properties of a voice channel.</exception>
        public override Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot modify text channel properties of a voice channel");

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Cannot create a thread within a voice channel.</exception>
        public override Task<RestThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null)
            => throw new InvalidOperationException("Cannot create a thread within a voice channel");

        #endregion

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";

        #region TextOverrides

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override Task<RestMessage> GetMessageAsync(ulong id, RequestOptions options = null)
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
        public override IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetMessagesAsync(fromMessage, dir, limit, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = 100, RequestOptions options = null)
        {
            if (!IsTextInVoice)
                throw new NotSupportedException("This function is only supported in Text-In-Voice channels");
            return base.GetMessagesAsync(limit, options);
        }

        /// <inheritdoc/> <exception cref="NotSupportedException">This function is only supported in Text-In-Voice channels.</exception>
        public override IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, RequestOptions options = null)
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


        #region IAudioChannel
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Connecting to a REST-based channel is not supported.</exception>
        Task<IAudioClient> IAudioChannel.ConnectAsync(bool selfDeaf, bool selfMute, bool external) { throw new NotSupportedException(); }
        Task IAudioChannel.DisconnectAsync() { throw new NotSupportedException(); }
        Task IAudioChannel.ModifyAsync(Action<AudioChannelProperties> func, RequestOptions options) { throw new NotSupportedException(); }
        #endregion

        #region IGuildChannel
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(null);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
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
