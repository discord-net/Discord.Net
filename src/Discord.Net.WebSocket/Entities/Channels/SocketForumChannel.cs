using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

            Tags = model.ForumTags.GetValueOrDefault(new API.ForumTags[0]).Select(
                x => new ForumTag(x.Id, x.Name, x.EmojiId.GetValueOrDefault(null), x.EmojiName.GetValueOrDefault())
            ).ToImmutableArray();
        }

        /// <inheritdoc cref="IForumChannel.CreatePostAsync(string, ThreadArchiveDuration, Message, int?, RequestOptions)"/>
        public Task<RestThreadChannel> CreatePostAsync(string title, ThreadArchiveDuration archiveDuration, Message message, int? slowmode = null, RequestOptions options = null)
            => ThreadHelper.CreatePostAsync(this, Discord, title, archiveDuration, message, slowmode, options);

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
        async Task<IThreadChannel> IForumChannel.CreatePostAsync(string title, ThreadArchiveDuration archiveDuration, Message message, int? slowmode, RequestOptions options)
            => await CreatePostAsync(title, archiveDuration, message, slowmode, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetActiveThreadsAsync(RequestOptions options)
            => await GetActiveThreadsAsync(options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetPublicArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetPublicArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetPrivateArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetPrivateArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IThreadChannel>> IForumChannel.GetJoinedPrivateArchivedThreadsAsync(int? limit, DateTimeOffset? before, RequestOptions options)
            => await GetJoinedPrivateArchivedThreadsAsync(limit, before, options).ConfigureAwait(false);
        #endregion
    }
}
