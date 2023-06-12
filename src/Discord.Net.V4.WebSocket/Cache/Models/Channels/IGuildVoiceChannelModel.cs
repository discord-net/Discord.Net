using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IGuildVoiceChannelModel : IGuildTextChannelModel, IAudioChannelModel
    {
        int Bitrate { get; }
        int? UserLimit { get; }
    }
}
