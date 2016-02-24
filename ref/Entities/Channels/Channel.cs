using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public abstract class Channel : IChannel
    {        
        public ulong Id { get; }

        public abstract DiscordClient Client { get; }
        public abstract ChannelType Type { get; }
        public bool IsText { get; }
        public bool IsVoice { get; }
        public bool IsPrivate { get; }
        public bool IsPublic { get; }

        public abstract User CurrentUser { get; }
        public abstract IEnumerable<User> Users { get; }

        public abstract Task Save();
    }
}
