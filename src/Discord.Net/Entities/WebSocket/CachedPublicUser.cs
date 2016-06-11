using Discord.Data;
using ChannelModel = Discord.API.Channel;
using Model = Discord.API.User;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedPublicUser : User, ICachedUser
    {
        private int _references;
        private Game? _game;
        private UserStatus _status;

        public CachedDMChannel DMChannel { get; private set; }

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public override UserStatus Status => _status;
        public override Game? Game => _game;

        public CachedPublicUser(DiscordSocketClient discord, Model model) 
            : base(discord, model)
        {
        }

        public CachedDMChannel AddDMChannel(ChannelModel model)
        {
            lock (this)
            {
                var channel = new CachedDMChannel(Discord, this, model);
                DMChannel = channel;
                return channel;
            }
        }
        public CachedDMChannel RemoveDMChannel(ulong id)
        {
            lock (this)
            {
                var channel = DMChannel;
                if (channel.Id == id)
                {
                    DMChannel = null;
                    return channel;
                }
                return null;
            }
        }

        public void Update(PresenceModel model, UpdateSource source)
        {
            if (source == UpdateSource.Rest) return;

            var game = model.Game != null ? new Game(model.Game) : (Game?)null;

            _status = model.Status;
            _game = game;
        }

        public void AddRef()
        {
            lock (this)
                _references++;
        }
        public void RemoveRef()
        {
            lock (this)
            {
                if (--_references == 0 && DMChannel == null)
                    Discord.RemoveUser(Id);
            }
        }

        public CachedPublicUser Clone() => MemberwiseClone() as CachedPublicUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
