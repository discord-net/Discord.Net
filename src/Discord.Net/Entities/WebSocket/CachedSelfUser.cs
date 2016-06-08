using Model = Discord.API.User;

namespace Discord
{
    internal class CachedSelfUser : SelfUser, ICachedEntity<ulong>
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;

        public CachedSelfUser(DiscordSocketClient discord, Model model) 
            : base(discord, model)
        {
        }

        public CachedSelfUser Clone() => MemberwiseClone() as CachedSelfUser;
    }
}
