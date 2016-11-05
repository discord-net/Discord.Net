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
            Emoji = Emoji.FromApi(model.Emoji);
        }

        public ulong UserId { get; private set; }
        public ulong MessageId { get; private set; }
        public ulong ChannelId { get; private set; }
        public Emoji Emoji { get; private set; }
    }
}
