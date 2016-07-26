using System;
using Model = Discord.API.User;

namespace Discord
{
    internal class SocketSelfUser : SelfUser, ISocketUser
    {
        internal override bool IsAttached => true;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        SocketGlobalUser ISocketUser.User { get { throw new NotSupportedException(); } }

        public SocketSelfUser(DiscordSocketClient discord, Model model) 
            : base(discord, model)
        {
        }

        public SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;
        ISocketUser ISocketUser.Clone() => Clone();
    }
}
