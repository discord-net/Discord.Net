using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IGuildChannelModel : IChannelModel
    {
        ulong? Parent { get; }
        int Position { get; }
        IOverwriteModel[] Permissions { get; }
    }
}
