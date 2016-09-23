using Discord.Rest;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    internal class SocketUserMessage : UserMessage, ISocketMessage
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new ISocketMessageChannel Channel => base.Channel as ISocketMessageChannel;

        public SocketUserMessage(ISocketMessageChannel channel, IUser author, Model model) 
            : base(channel, author, model)
        {
        }

        public ISocketMessage Clone() => MemberwiseClone() as ISocketMessage;
    }
}
