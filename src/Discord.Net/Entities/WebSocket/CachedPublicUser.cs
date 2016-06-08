using ChannelModel = Discord.API.Channel;
using Model = Discord.API.User;

namespace Discord
{
    internal class CachedPublicUser : User, ICachedEntity<ulong>
    {
        private int _references;

        public CachedDMChannel DMChannel { get; private set; }

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;

        public CachedPublicUser(DiscordSocketClient discord, Model model) 
            : base(discord, model)
        {
        }

        public CachedDMChannel SetDMChannel(ChannelModel model)
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
                    Discord.RemoveCachedUser(Id);
            }
        }

        public CachedPublicUser Clone() => MemberwiseClone() as CachedPublicUser;
    }
}
