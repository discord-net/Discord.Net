using Discord.Gateway.Cache;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway
{
    public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketNestedChannel
    {
        public bool IsNsfw
            => _source.IsNsfw;

        public string? Topic
            => _source.Topic;

        public virtual int SlowModeInterval
            => _source.Slowmode;

        public ThreadArchiveDuration DefaultArchiveDuration
            => _source.DefaultArchiveDuration;

        public string Mention
            => MentionUtils.MentionChannel(Id);

        public ulong? CategoryId
            => _source.Parent;

        public GuildChannelCacheable Parent { get; }


        protected override IGuildTextChannelModel Model
            => _source;

        public int DefaultSlowModeInterval => throw new NotImplementedException(); // TODO

        private IGuildTextChannelModel _source;

        public SocketTextChannel(DiscordGatewayClient discord, ulong guildId, IGuildTextChannelModel model)
            : base(discord, guildId, model)
        {
            Update(model);

            Parent = new(() => _source.Parent.ToOptional(), Discord, discord.State.GuildChannels.SourceSpecific);
        }

        [MemberNotNull(nameof(_source))]
        internal void Update(IGuildTextChannelModel model)
        {
            _source = model;
        }

        internal override void Update(IChannelModel model)
        {
            if(model is IGuildTextChannelModel guildTextChannelModel)
            {
                Update(guildTextChannelModel);
            }
        }

        public Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, IMessage? message = null, bool? invitable = null, int? slowmode = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IWebhook> CreateWebhookAsync(string name, Stream? avatar = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task DeleteMessageAsync(ulong messageId, RequestOptions? options = null) => throw new NotImplementedException();
        public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) => throw new NotImplementedException();
        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions? options = null) => throw new NotImplementedException();
        public Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions? options = null) => throw new NotImplementedException();
        public IDisposable EnterTypingState(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IUserMessage> ModifyMessageAsync(ulong messageId, Action<MessageProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IUserMessage> SendFileAsync(string filePath, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IUserMessage> SendFileAsync(Stream stream, string filename, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IUserMessage> SendFileAsync(FileAttachment attachment, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IUserMessage> SendFilesAsync(IEnumerable<FileAttachment> attachments, string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IUserMessage> SendMessageAsync(string? text = null, bool isTTS = false, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task SyncPermissionsAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task TriggerTypingAsync(RequestOptions? options = null) => throw new NotImplementedException();
        
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null) => throw new NotImplementedException();
    }
}

