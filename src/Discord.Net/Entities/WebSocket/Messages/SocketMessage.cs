using Model = Discord.API.Message;

namespace Discord
{
    internal class SocketMessage : Message
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new ISocketMessageChannel Channel => base.Channel as ISocketMessageChannel;

        public SocketMessage(ISocketMessageChannel channel, IUser author, Model model) 
            : base(channel, author, model)
        {
        }

        public SocketMessage Clone() => MemberwiseClone() as SocketMessage;
    }
}
