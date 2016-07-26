using Model = Discord.API.Message;

namespace Discord
{
    internal class CachedMessage : Message
    {
        bool IEntity<ulong>.IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new ICachedMessageChannel Channel => base.Channel as ICachedMessageChannel;

        public CachedMessage(ICachedMessageChannel channel, IUser author, Model model) 
            : base(channel, author, model)
        {
        }

        public CachedMessage Clone() => MemberwiseClone() as CachedMessage;
    }
}
