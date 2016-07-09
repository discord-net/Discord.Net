using System;
using Model = Discord.API.User;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedGlobalUser : User, ICachedUser
    {
        private ushort _references;

        public Presence Presence { get; private set; }

        public new DiscordSocketClient Discord { get { throw new NotSupportedException(); } }

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
        public void Update(PresenceModel model, UpdateSource source)
        {
            //Race conditions are okay here. Multiple shards racing already cant guarantee presence in order.

            //lock (this)
            //{
                var game = model.Game != null ? new Game(model.Game) : null;
                Presence = new Presence(game, model.Status);
            //}
        }

        public CachedGlobalUser Clone() => MemberwiseClone() as CachedGlobalUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
