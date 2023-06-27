using Discord.WebSocket.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public class DiscordSocketClient : IGatewayClient
    {
        internal StateController State { get; }

        public DiscordSocketClient()
        {
            State = new (this);
        }
    }
}
