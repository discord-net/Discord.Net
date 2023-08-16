using Discord.Gateway.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public class DiscordGatewayClient : IGatewayClient
    {
        internal StateController State { get; }

        public DiscordGatewayClient()
        {
            State = new (this);
        }
    }
}
