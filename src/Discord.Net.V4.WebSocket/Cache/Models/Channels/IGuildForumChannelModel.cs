using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IGuildForumChannelModel : IGuildChannelModel
    {
        bool IsNsfw { get; }
        string? Topic { get; }
        ThreadArchiveDuration DefaultAutoArchiveDuration { get; }
        IForumTagModel[] Tags { get; }
    }

    public interface IForumTagModel
    {
        ulong Id { get; }
        string Name { get; }
        IEmojiModel? Emote { get; }
    }
}
