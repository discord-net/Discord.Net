using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IGroupDMChannelModel : IAudioChannelModel, IChannelModel
    {
        ulong[] Recipients { get; }
    }
}
