using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based forum channel in a guild.
    /// </summary>
    public class RestForumChannel : RestGuildChannel, IForumChannel
    {
        /// <inheritdoc/>
        public bool IsNsfw { get; private set; }

        /// <inheritdoc/>
        public string Topic { get; private set; }

        /// <inheritdoc/>
        public ThreadArchiveDuration DefaultAutoArchiveDuration { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ForumTag> Tags { get; private set; }

        /// <inheritdoc/>
        public int ThreadCreationInterval { get; private set; }

        /// <inheritdoc/>
        public int DefaultSlowModeInterval { get; private set; }

        /// <inheritdoc/>
        public ulong? CategoryId { get; private set; }

        /// <inheritdoc/>
        public IEmote DefaultReactionEmoji { get; private set; }

        /// <inheritdoc/>
        public ForumSortOrder? DefaultSortOrder { get; private set; }

        /// <inheritdoc />
        public ForumLayout DefaultLayout { get; private set; }

        /// <inheritdoc/>
        public string Mention => MentionUtils.MentionChannel(Id);

        internal RestForumChannel(BaseDiscordClient client, IGuild guild, ulong id, ulong guildId)
            : base(client, guild, id, guildId)
        {

        }

        internal new static RestForumChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestForumChannel(discord, guild, model.Id, guild?.Id ?? model.GuildId.Value);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);
            IsNsfw = model.Nsfw.GetValueOrDefault(false);
            Topic = model.Topic.GetValueOrDefault();
            DefaultAutoArchiveDuration = model.AutoArchiveDuration.GetValueOrDefault(ThreadArchiveDuration.OneDay);

            if (model.ThreadRateLimitPerUser.IsSpecified)
                DefaultSlowModeInterval = model.ThreadRateLimitPerUser.Value;

            if (model.SlowMode.IsSpecified)
                ThreadCreationInterval = model.SlowMode.Value;

            DefaultSortOrder = model.DefaultSortOrder.GetValueOrDefault();

            Tags = model.ForumTags.GetValueOrDefault(Array.Empty<API.ForumTag>()).Select(
                x => new ForumTag(x.Id, x.Name, x.EmojiId.GetValueOrDefault(null), x.EmojiName.GetValueOrDefault(), x.Moderated)
            ).ToImmutableArray();

            if (model.DefaultReactionEmoji.IsSpecified && model.DefaultReactionEmoji.Value is not null)
            {
                if (model.DefaultReactionEmoji.Value.EmojiId.HasValue && model.DefaultReactionEmoji.Value.EmojiId.Value != 0)
                    DefaultReactionEmoji = new Emote(model.DefaultReactionEmoji.Value.EmojiId.GetValueOrDefault(), null, false);
                else if (model.DefaultReactionEmoji.Value.EmojiName.IsSpecified)
                    DefaultReactionEmoji = new Emoji(model.DefaultReactionEmoji.Value.EmojiName.Value);
                else
                    DefaultReactionEmoji = null;
            }

            CategoryId = model.CategoryId.GetValueOrDefault();
            DefaultLayout = model.DefaultForumLayout.GetValueOrDefault();
        }

        /// <inheritdoc/>
        public async Task ModifyAsync(Action<ForumChannelProperties> func, RequestOptions options = null)
        {
            var model = await ForumHelper.ModifyAsync(this, Discord, func, options);
            Update(model);
        }

        /// <inheritdoc cref="IForumChannel.CreatePostAsync(string, ThreadArchiveDuration, int?, string, Embed, RequestOptions, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags, ForumTag[])"/>
        public Task<RestThreadChannel> CreatePostAsync(string title, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null,
            string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null,
            Embed[] embeds = null, MessageFlags flags = MessageFlags.None, ForumTag[] tags = null)
            => ThreadHelper.CreatePostAsync(this, Discord, title, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags?.Select(tag => tag.Id).ToArray());

        /// <inheritdoc cref="IForumChannel.CreatePostWithFileAsync(string, string, ThreadArchiveDuration, int?, string, Embed, RequestOptions, bool, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags, ForumTag[])"/>
        public async Task<RestThreadChannel> CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, ForumTag[] tags = null)
        {
            using var file = new FileAttachment(filePath, isSpoiler: isSpoiler);
            return await ThreadHelper.CreatePostAsync(this, Discord, title, new FileAttachment[] { file }, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags?.Select(tag => tag.Id).ToArray()).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IForumChannel.CreatePostWithFileAsync(string, Stream, string, ThreadArchiveDuration, int?, string, Embed, RequestOptions, bool, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags, ForumTag[])"/>
        public async Task<RestThreadChannel> CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, ForumTag[] tags = null)
        {
            using var file = new FileAttachment(stream, filename, isSpoiler: isSpoiler);
            return await ThreadHelper.CreatePostAsync(this, Discord, title, new FileAttachment[] { file }, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags?.Select(tag => tag.Id).ToArray()).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IForumChannel.CreatePostWithFileAsync(string, FileAttachment, ThreadArchiveDuration, int?, string, Embed, RequestOptions, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags, ForumTag[])"/>
        public Task<RestThreadChannel> CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, ForumTag[] tags = null)
            => ThreadHelper.CreatePostAsync(this, Discord, title, new FileAttachment[] { attachment }, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags?.Select(tag => tag.Id).ToArray());

        /// <inheritdoc cref="IForumChannel.CreatePostWithFilesAsync(string, IEnumerable{FileAttachment}, ThreadArchiveDuration, int?, string, Embed, RequestOptions, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags, ForumTag[])"/>
        public Task<RestThreadChannel> CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None, ForumTag[] tags = null)
            => ThreadHelper.CreatePostAsync(this, Discord, title, attachments, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags?.Select(tag => tag.Id).ToArray());

        /// <inheritdoc cref="ITextChannel.GetActiveThreadsAsync(RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
            => ThreadHelper.GetActiveThreadsAsync(Guild, Id, Discord, options);

        /// <inheritdoc cref="IForumChannel.GetJoinedPrivateArchivedThreadsAsync(int?, DateTimeOffset?, RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null)
            => ThreadHelper.GetJoinedPrivateArchivedThreadsAsync(this, Discord, limit, before, options);

        /// <inheritdoc cref="IForumChannel.GetPrivateArchivedThreadsAsync(int?, DateTimeOffset?, RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null)
            => ThreadHelper.GetPrivateArchivedThreadsAsync(this, Discord, limit, before, options);

        /// <inheritdoc cref="IForumChannel.GetPublicArchivedThreadsAsync(int?, DateTimeOffset?, RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetPublicArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null)
            => ThreadHelper.GetPublicArchivedThreadsAsync(this, Discord, limit, before, options);

        /// <inheritdoc cref="IIntegrationChannel.CreateWebhookAsync"/>
        public Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
            => ChannelHelper.CreateWebhookAsync(this, Discord, name, avatar, options);

        /// <inheritdoc cref="IIntegrationChannel.GetWebhookAsync"/>
        public Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetWebhookAsync(this, Discord, id, options);

        /// <inheritdoc cref="IIntegrationChannel.GetWebhooksAsync"/>
        public Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => ChannelHelper.GetWebhooksAsync(this, Discord, options);

        #region IForumChannel
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetActiveThreadsAsync(RequestOptions options)
            => await GetActiveThreadsAsync(options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetPublicArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetPublicArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetPrivateArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetPrivateArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetJoinedPrivateArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetJoinedPrivateArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostAsync(string title, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, ForumTag[] tags)
            => await CreatePostAsync(title, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, ForumTag[] tags)
            => await CreatePostWithFileAsync(title, filePath, archiveDuration, slowmode, text, embed, options, isSpoiler, allowedMentions, components, stickers, embeds, flags, tags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, ForumTag[] tags)
            => await CreatePostWithFileAsync(title, stream, filename, archiveDuration, slowmode, text, embed, options, isSpoiler, allowedMentions, components, stickers, embeds, flags, tags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, ForumTag[] tags)
            => await CreatePostWithFileAsync(title, attachment, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags, ForumTag[] tags)
            => await CreatePostWithFilesAsync(title, attachments, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags, tags);

        #endregion

        #region INestedChannel
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

        /// <inheritdoc />
        async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        {
            if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
                return (await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false)) as ICategoryChannel;
            return null;
        }

        /// <inheritdoc />
        public Task SyncPermissionsAsync(RequestOptions options = null)
            => ChannelHelper.SyncPermissionsAsync(this, Discord, options);
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

    }
}
