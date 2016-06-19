using ChannelModel = Discord.API.Channel;
using Model = Discord.API.User;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedPublicUser : User, ICachedUser
    {
        private int _references;
        //private Game? _game;
        //private UserStatus _status;

        public CachedDMChannel DMChannel { get; private set; }

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public override UserStatus Status => UserStatus.Unknown;// _status;
        public override Game Game => null; //_game;

        public CachedPublicUser(Model model) 
            : base(model)
        {
        }

        public CachedDMChannel AddDMChannel(DiscordSocketClient discord, ChannelModel model)
        {
            lock (this)
            {
                var channel = new CachedDMChannel(discord, this, model);
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

            //var game = model.Game != null ? new Game(model.Game) : (Game)null;

            //_status = model.Status;
            //_game = game;
        }

        public void AddRef()
        {
            lock (this)
                _references++;
        }
        public void RemoveRef(DiscordSocketClient discord)
        {
            lock (this)
            {
                if (--_references == 0 && DMChannel == null)
                    discord.RemoveUser(Id);
            }
        }

        public CachedPublicUser Clone() => MemberwiseClone() as CachedPublicUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
