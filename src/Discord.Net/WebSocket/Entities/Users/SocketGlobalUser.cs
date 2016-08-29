using Discord.Rest;
using System;
using Model = Discord.API.User;
using PresenceModel = Discord.API.Presence;

namespace Discord.WebSocket
{
    internal class SocketGlobalUser : User, ISocketUser
    {
        internal override bool IsAttached => true;
        private readonly object _lockObj = new object();

        private ushort _references;

        public Presence Presence { get; private set; }

        public new DiscordSocketClient Discord { get { throw new NotSupportedException(); } }
        SocketGlobalUser ISocketUser.User => this;

        public SocketGlobalUser(Model model) 
            : base(model)
        {
        }

        public void AddRef()
        {
            checked
            {
                lock (_lockObj)
                    _references++;
            }
        }
        public void RemoveRef(DiscordSocketClient discord)
        {
            lock (_lockObj)
            {
                if (--_references == 0)
                    discord.RemoveUser(Id);
            }
        }

        public override void Update(Model model, UpdateSource source)
        {
            lock (_lockObj)
                base.Update(model, source);
        }
        public void Update(PresenceModel model, UpdateSource source)
        {
            //Race conditions are okay here. Multiple shards racing already cant guarantee presence in order.

            //lock (_lockObj)
            //{
                var game = model.Game != null ? new Game(model.Game) : null;
                Presence = new Presence(game, model.Status);
            //}
        }

        public SocketGlobalUser Clone() => MemberwiseClone() as SocketGlobalUser;
        ISocketUser ISocketUser.Clone() => Clone();
    }
}
