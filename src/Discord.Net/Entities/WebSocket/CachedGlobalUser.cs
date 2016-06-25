using System;
using Discord.API;
using Model = Discord.API.User;

namespace Discord
{
    internal class CachedGlobalUser : User, ICachedUser
    {
        private ushort _references;

        public new DiscordSocketClient Discord { get { throw new NotSupportedException(); } }
        public override UserStatus Status => UserStatus.Unknown;// _status;
        public override Game Game => null; //_game;

        public CachedGlobalUser(Model model) 
            : base(model)
        {
        }

        public void AddRef()
        {
            checked
            {
                lock (this)
                    _references++;
            }
        }
        public void RemoveRef(DiscordSocketClient discord)
        {
            lock (this)
            {
                if (--_references == 0)
                    discord.RemoveUser(Id);
            }
        }

        public override void Update(Model model, UpdateSource source)
        {
            lock (this)
                base.Update(model, source);
        }

        public CachedGlobalUser Clone() => MemberwiseClone() as CachedGlobalUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
