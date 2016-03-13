using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public class Message : IEntity<ulong>
    {
        public class Attachment : File
        {
            public string Id { get; }
            public int Size { get; }
            public string Filename { get; }
        }

        public class Embed
        {
            public string Url { get; }
            public string Type { get; }
            public string Title { get; }
            public string Description { get; }
            public EmbedLink Author { get; }
            public EmbedLink Provider { get; }
            public File Thumbnail { get; }
            public File Video { get; }
        }

        public class EmbedLink
        {
            public string Url { get; }
            public string Name { get; }
        }

        public class File
        {
            public string Url { get; }
            public string ProxyUrl { get; }
            public int? Width { get; }
            public int? Height { get; }
        }
  
        public ulong Id { get; }
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public ITextChannel Channel { get; }
        public IUser User { get; }        
        public bool IsTTS { get; }
        public string RawText { get; }
        public string Text { get; }
        public DateTime Timestamp { get; }
        public DateTime? EditedTimestamp { get; }
        public Attachment[] Attachments { get; }
        public Embed[] Embeds { get; }
        
        public IReadOnlyList<ServerUser> MentionedUsers { get; }
        public IReadOnlyList<IPublicChannel> MentionedChannels { get; }
        public IReadOnlyList<Role> MentionedRoles { get; }
        
        public Server Server => null;
        public bool IsAuthor => false;
        
        public bool IsMentioningMe(bool includeRoles = false) => false;

        public Task Update() => null;
        public Task Delete() => null;
    }
}
