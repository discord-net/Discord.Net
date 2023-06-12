using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.V4.WebSocket.Cache.Models.Channels
{
    public interface IGroupDMChannelModel : IAudioChannelModel
    {
        ulong[] Recipients { get; }
    }
}
