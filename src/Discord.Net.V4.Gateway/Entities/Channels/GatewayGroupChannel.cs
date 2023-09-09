using Discord.Audio;
using Discord.Entities.Messages.Embeds;
using Discord.Gateway.Cache;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    internal sealed class GatewayGroupChannel : GatewayCacheableEntity<ulong, IGroupDMChannelModel>, IGatewayMessageChannel, IGroupChannel
    {
        public UsersCacheable Recipients { get; }
        public MessagesCacheable CachedMessages { get; }

        public string? RTCRegion
            => _source.RTCRegion;

        protected override IGroupDMChannelModel Model
            => _source;

        public string Name
            => _source.Name;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        private IGroupDMChannelModel _source;

        public GatewayGroupChannel(DiscordGatewayClient discord, IGroupDMChannelModel model)
            : base(discord, model.Id)
        {
            Update(model);

            Recipients = new UsersCacheable(
                () => _source.Recipients,
                id => new UserCacheable(id, discord, Discord.State.Users.ProvideSpecific(id))
            );

            CachedMessages = new MessagesCacheable(
                Id,
                discord.State.Messages,
                messageId => new MessageCacheable(messageId, discord, discord.State.Messages.ProvideSpecific(messageId))
            );
        }

        [MemberNotNull(nameof(_source))]
        internal override void Update(IGroupDMChannelModel model)
        {
            _source = model;
        }

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
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        IChannelModel ICacheableEntity<ulong, IChannelModel>.GetModel() => throw new NotImplementedException();
        public Task LeaveAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false) => throw new NotImplementedException();
        public Task DisconnectAsync() => throw new NotImplementedException();
        public Task ModifyAsync(Action<ModifyAudioChannelProperties> func, RequestOptions? options = null) => throw new NotImplementedException();

        IEntityEnumerableSource<IUser, ulong> IPrivateChannel.Recipients => Recipients;
    }
}
