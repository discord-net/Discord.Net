using Discord.Entities.Channels.Threads;
using Discord.Entities.Messages.AllowedMentions;
using Discord.Entities.Messages.Embeds;
using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public sealed class GatewayForumChannel : GatewayGuildChannel, IForumChannel
    {
        public GuildCategoryChannelCacheable? Category { get; private set; }

        public bool IsNsfw
            => _source.IsNsfw;

        public string? Topic
            => _source.Topic;

        public ThreadArchiveDuration DefaultAutoArchiveDuration
            => _source.DefaultAutoArchiveDuration;

        public IReadOnlyCollection<IForumTag> Tags => throw new NotImplementedException(); // TODO

        public string Mention
            => MentionUtils.MentionChannel(Id);

        protected override IGuildChannelModel Model
            => _source;

        public int ThreadCreationInterval => throw new NotImplementedException();

        public int DefaultSlowModeInterval => throw new NotImplementedException();

        public IEmote DefaultReactionEmoji => throw new NotImplementedException();

        public ForumSortOrder? DefaultSortOrder => throw new NotImplementedException();

        public ForumLayout DefaultLayout => throw new NotImplementedException();

        private IGuildForumChannelModel _source;

        public GatewayForumChannel(DiscordGatewayClient discord, ulong guildId, IGuildForumChannelModel model)
            : base(discord, guildId, model)
        {
            Update(model);
        }


        [MemberNotNull(nameof(_source))]
        internal void Update(IGuildForumChannelModel model)
        {
            _source = model;

            Category = EntityUtils.UpdateCacheableFrom(
                Discord,
                Category,
                Discord.State.CategoryChannels,
                model.ParentId,
                Guild.Id
            );
        }

        internal override void Update(IChannelModel model)
        {
            if (model is IGuildForumChannelModel forumModel)
                Update(forumModel);
        }

        public Task ModifyAsync(Action<ModifyForumChannelProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostAsync(string title, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string? text = null, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, IForumTag[]? tags = null) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string? text = null, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, IForumTag[]? tags = null) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string? text = null, Embed? embed = null, RequestOptions? options = null, bool isSpoiler = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, IForumTag[]? tags = null) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string? text = null, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, IForumTag[]? tags = null) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string? text = null, Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null, MessageComponent? components = null, ISticker[]? stickers = null, Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, IForumTag[]? tags = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetPublicArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task SyncPermissionsAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteToApplicationAsync(ulong applicationId, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteToApplicationAsync(DefaultApplications application, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IInviteMetadata> CreateInviteToStreamAsync(IUser user, int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IWebhook> CreateWebhookAsync(string name, Stream? avatar = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions? options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        IEntitySource<ICategoryChannel, ulong>? INestedChannel.Category => Category;
    }
}
