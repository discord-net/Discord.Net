using System;
using Model = Discord.API.User;

namespace Discord
{
    internal class CachedSelfUser : SelfUser, ICachedUser
    {
        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        CachedGlobalUser ICachedUser.User { get { throw new NotSupportedException(); } }

        public CachedSelfUser(DiscordSocketClient discord, Model model) 
            : base(discord, model)
        {
        }

        public CachedSelfUser Clone() => MemberwiseClone() as CachedSelfUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
