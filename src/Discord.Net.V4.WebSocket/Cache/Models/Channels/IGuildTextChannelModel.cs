using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IGuildTextChannelModel : IGuildChannelModel
    {
        bool IsNsfw { get; }
        string? Topic { get; }
        int Slowmode { get; }
        ThreadArchiveDuration DefaultArchiveDuration { get; }
    }
}
