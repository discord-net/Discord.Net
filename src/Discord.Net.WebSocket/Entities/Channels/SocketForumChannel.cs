using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a forum channel in a guild.
    /// </summary>
    public class SocketForumChannel : SocketGuildChannel, IForumChannel
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
        public string Mention => MentionUtils.MentionChannel(Id);

        internal SocketForumChannel(DiscordSocketClient discord, ulong id, SocketGuild guild) : base(discord, id, guild) { }

        internal new static SocketForumChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketForumChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);
            IsNsfw = model.Nsfw.GetValueOrDefault(false);
            Topic = model.Topic.GetValueOrDefault();
            DefaultAutoArchiveDuration = model.AutoArchiveDuration.GetValueOrDefault(ThreadArchiveDuration.OneDay);

            Tags = model.ForumTags.GetValueOrDefault(Array.Empty<API.ForumTags>()).Select(
                x => new ForumTag(x.Id, x.Name, x.EmojiId.GetValueOrDefault(null), x.EmojiName.GetValueOrDefault())
            ).ToImmutableArray();
        }

        /// <inheritdoc cref="IForumChannel.CreatePostAsync(string, ThreadArchiveDuration, int?, string, Embed, RequestOptions, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags)"/>
        public Task<RestThreadChannel> CreatePostAsync(string title, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
            => ThreadHelper.CreatePostAsync(this, Discord, title, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags);

        /// <inheritdoc cref="IForumChannel.CreatePostWithFileAsync(string, string, ThreadArchiveDuration, int?, string, Embed, RequestOptions, bool, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags)"/>
        public async Task<RestThreadChannel> CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            using var file = new FileAttachment(filePath, isSpoiler: isSpoiler);
            return await ThreadHelper.CreatePostAsync(this, Discord, title, new FileAttachment[] { file }, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IForumChannel.CreatePostWithFileAsync(string, Stream, string, ThreadArchiveDuration, int?, string, Embed, RequestOptions, bool, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags)"/>
        public async Task<RestThreadChannel> CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null,
            ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
        {
            using var file = new FileAttachment(stream, filename, isSpoiler: isSpoiler);
            return await ThreadHelper.CreatePostAsync(this, Discord, title, new FileAttachment[] { file }, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IForumChannel.CreatePostWithFileAsync(string, FileAttachment, ThreadArchiveDuration, int?, string, Embed, RequestOptions, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags)"/>
        public Task<RestThreadChannel> CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
            => ThreadHelper.CreatePostAsync(this, Discord, title, new FileAttachment[] { attachment }, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags);

        /// <inheritdoc cref="IForumChannel.CreatePostWithFilesAsync(string, IEnumerable{FileAttachment}, ThreadArchiveDuration, int?, string, Embed, RequestOptions, AllowedMentions, MessageComponent, ISticker[], Embed[], MessageFlags)"/>
        public Task<RestThreadChannel> CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
            int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null,
            MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None)
            => ThreadHelper.CreatePostAsync(this, Discord, title, attachments, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags);

        /// <inheritdoc cref="IForumChannel.GetActiveThreadsAsync(RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null)
            => ThreadHelper.GetActiveThreadsAsync(Guild, Discord, options);

        /// <inheritdoc cref="IForumChannel.GetJoinedPrivateArchivedThreadsAsync(int?, DateTimeOffset?, RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null)
            => ThreadHelper.GetJoinedPrivateArchivedThreadsAsync(this, Discord, limit, before, options);

        /// <inheritdoc cref="IForumChannel.GetPrivateArchivedThreadsAsync(int?, DateTimeOffset?, RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null)
            => ThreadHelper.GetPrivateArchivedThreadsAsync(this, Discord, limit, before, options);

        /// <inheritdoc cref="IForumChannel.GetPublicArchivedThreadsAsync(int?, DateTimeOffset?, RequestOptions)"/>
        public Task<IReadOnlyCollection<RestThreadChannel>> GetPublicArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null)
            => ThreadHelper.GetPublicArchivedThreadsAsync(this, Discord, limit, before, options);

        #region IForumChannel
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetActiveThreadsAsync(RequestOptions options)
            => await GetActiveThreadsAsync(options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetPublicArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetPublicArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetPrivateArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetPrivateArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetJoinedPrivateArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetJoinedPrivateArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostAsync(string title, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags)
            => await CreatePostAsync(title, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags)
            => await CreatePostWithFileAsync(title, filePath, archiveDuration, slowmode, text, embed, options, isSpoiler, allowedMentions, components, stickers, embeds, flags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, bool isSpoiler, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags)
            => await CreatePostWithFileAsync(title, stream, filename, archiveDuration, slowmode, text, embed, options, isSpoiler, allowedMentions, components, stickers, embeds, flags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags)
            => await CreatePostWithFileAsync(title, attachment, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags).ConfigureAwait(false);
        async Task<IThreadChannel> IForumChannel.CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration, int? slowmode, string text, Embed embed, RequestOptions options, AllowedMentions allowedMentions, MessageComponent components, ISticker[] stickers, Embed[] embeds, MessageFlags flags)
            => await CreatePostWithFilesAsync(title, attachments, archiveDuration, slowmode, text, embed, options, allowedMentions, components, stickers, embeds, flags);

        #endregion
    }
}
