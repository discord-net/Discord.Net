using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal sealed class SocketDMChannel : SocketChannel, 
    {
        public SocketDMChannel(DiscordSocketClient discord, ulong id, IChannelModel model) : base(discord, id, model)
        {
        }

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
    }
}
