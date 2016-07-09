using Model = Discord.API.Message;

namespace Discord
{
    internal class CachedMessage : Message, ICachedEntity<ulong>
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new ICachedMessageChannel Channel => base.Channel as ICachedMessageChannel;

        public CachedMessage(ICachedMessageChannel channel, IUser author, Model model) 
            : base(channel, author, model)
        {
        }

        public CachedMessage Clone() => MemberwiseClone() as CachedMessage;
    }
}
