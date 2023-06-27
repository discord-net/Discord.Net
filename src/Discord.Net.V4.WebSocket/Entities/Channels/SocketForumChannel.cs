using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public sealed class SocketForumChannel : SocketGuildChannel, IForumChannel
    {
        public bool IsNsfw
            => _source.IsNsfw;

        public string? Topic
            => _source.Topic;

        public ThreadArchiveDuration DefaultAutoArchiveDuration
            => _source.DefaultAutoArchiveDuration;

        public IReadOnlyCollection<ForumTag> Tags => throw new NotImplementedException(); // TODO

        public string Mention
            => MentionUtils.MentionChannel(Id);

        protected override IGuildChannelModel Model
            => _source;

        private IGuildForumChannelModel _source;

        public SocketForumChannel(DiscordSocketClient discord, ulong guildId, ulong id, IGuildForumChannelModel model) : base(discord, guildId, id, model)
        {
            _source = model;
        }

        internal override void Update(IChannelModel model)
        {
            if (model is IGuildForumChannelModel forumModel)
                _source = forumModel;
        }

        public Task<IThreadChannel> CreatePostAsync(string title, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, string filePath, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, Stream stream, string filename, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFileAsync(string title, FileAttachment attachment, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IThreadChannel> CreatePostWithFilesAsync(string title, IEnumerable<FileAttachment> attachments, ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay, int? slowmode = null, string text = null, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null, MessageFlags flags = MessageFlags.None) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null) => throw new NotImplementedException();
        public Task<IReadOnlyCollection<IThreadChannel>> GetPublicArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
        
    }
}
