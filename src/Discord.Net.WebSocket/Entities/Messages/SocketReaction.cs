using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.GatewayReaction;

namespace Discord.WebSocket
{
    public class SocketReaction : IReaction
    {
        internal SocketReaction(Model model)
        {
            UserId = model.UserId;
            MessageId = model.MessageId;
            ChannelId = model.ChannelId;
            Emoji = model.Emoji;
        }

        public ulong UserId { get; internal set; }
        public ulong MessageId { get; internal set; }
        public ulong ChannelId { get; internal set; }
        public API.Emoji Emoji { get; internal set; }
    }
}
