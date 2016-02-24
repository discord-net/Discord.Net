using System.Collections.Generic;

namespace Discord
{
    public interface IChannel : IModel<ulong>
    {
        DiscordClient Client { get; }
        
        ChannelType Type { get; }
        bool IsText { get; }
        bool IsVoice { get; }
        bool IsPrivate { get; }
        bool IsPublic { get; }
        
        IEnumerable<User> Users { get; }
    }
}
