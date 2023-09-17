using Discord.Rest;
using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Entities.Messages.Embeds;
using Discord.Entities.Messages.AllowedMentions;

namespace Discord.Gateway
{
    internal sealed class GatewayDMChannel : GatewayChannel, IGatewayMessageChannel, IDMChannel
    {
        public UserCacheable Recipient { get; }

        public MessagesCacheable CachedMessages { get; }

        protected override IDMChannelModel Model
            => _source;

        private IDMChannelModel _source;

        public GatewayDMChannel(DiscordGatewayClient discord, IDMChannelModel model)
            : base(discord, model)
        {
            _source = model;

            Recipient = new(
                model.RecipientId,
                discord,
                discord.State.Users.ProvideSpecific(model.RecipientId)
            );

            CachedMessages = new(Id, discord.State.Messages, messageId => new MessageCacheable(messageId, Discord, Discord.State.Messages.ProvideSpecific(messageId)));
        }


        internal override void Update(IChannelModel model)
        {
            if (model is IDMChannelModel dmChannelModel)
                _source = dmChannelModel;
        }

        public Task CloseAsync(RequestOptions? options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        public Task<RestUserMessage> SendMessageAsync(string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<RestUserMessage> SendFileAsync(string filePath, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<RestUserMessage> SendFileAsync(FileAttachment attachment, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<RestUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public MessageCacheable GetCachedMessage(ulong id) => throw new NotImplementedException();
        public MessagesCacheable GetCachedMessages(int limit = 100) => throw new NotImplementedException();
        public MessagesCacheable GetCachedMessages(ulong fromMessageId, Direction dir, int limit = 100) => throw new NotImplementedException();
        public MessagesCacheable GetCachedMessages(IMessage fromMessage, Direction dir, int limit = 100) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) => throw new NotImplementedException();
        Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS, Embed embed, RequestOptions? options, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags) => throw new NotImplementedException();
        Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS, Embed embed, RequestOptions? options, bool isSpoiler, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags) => throw new NotImplementedException();
        Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS, Embed embed, RequestOptions? options, bool isSpoiler, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags) => throw new NotImplementedException();
        Task<IUserMessage> IMessageChannel.SendFileAsync(FileAttachment attachment, string text, bool isTTS, Embed embed, RequestOptions? options, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags) => throw new NotImplementedException();
        Task<IUserMessage> IMessageChannel.SendFilesAsync(IEnumerable<FileAttachment> attachments, string text, bool isTTS, Embed embed, RequestOptions? options, AllowedMentions allowedMentions, MessageReference messageReference, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags) => throw new NotImplementedException();
        public Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync(RequestOptions? options) => throw new NotImplementedException();
        public Task DeleteMessageAsync(ulong messageId, RequestOptions? options = null) => throw new NotImplementedException();
        public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task TriggerTypingAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public IDisposable EnterTypingState(RequestOptions? options = null) => throw new NotImplementedException();


        IEntitySource<IUser, ulong> IDMChannel.Recipient
            => Recipient;

        public IEntityEnumerableSource<IUser, ulong> Recipients => throw new NotImplementedException(); // TOD
    }
}
