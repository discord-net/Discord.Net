using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IReactionModel
    {
        int Count { get; }
        bool Me { get; }
        ulong? EmojiId { get; }
        string? EmojiName { get; }
    }
}
