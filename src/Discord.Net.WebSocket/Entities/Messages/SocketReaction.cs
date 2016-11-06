using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.GatewayReaction;

namespace Discord.WebSocket
{
    public class SocketReaction : IReaction
    {
        internal SocketReaction(Model model, ISocketMessageChannel channel, Optional<SocketUserMessage> message, Optional<IUser> user)
        {
            Channel = channel;
            Message = message;
            MessageId = model.MessageId;
            User = user;
            UserId = model.UserId;
            Emoji = Emoji.FromApi(model.Emoji);
        }

        public ulong UserId { get; private set; }
        public Optional<IUser> User { get; private set; }
        public ulong MessageId { get; private set; }
        public Optional<SocketUserMessage> Message { get; private set; }
        public ISocketMessageChannel Channel { get; private set; }
        public Emoji Emoji { get; private set; }
    }
}
