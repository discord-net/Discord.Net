using APIChannel = Discord.API.Client.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.API.Client.Rest;

namespace Discord
{
    public class VoiceChannel : PublicChannel, IPublicChannel, IVoiceChannel
    {
        private readonly static Action<VoiceChannel, VoiceChannel> _cloner = DynamicIL.CreateCopyMethod<VoiceChannel>();

        public int Bitrate { get; set; }

        public override ChannelType Type => ChannelType.Public | ChannelType.Voice;
        /// <summary> Gets a collection of all users currently in this voice channel. </summary>
        public override IEnumerable<User> Users
        {
            get
            {
                if (Client.Config.UsePermissionsCache)
                    return _permissions.Users.Select(x => x.User).Where(x => x.VoiceChannel == this);
                else
                    return Server.Users.Where(x => x.VoiceChannel == this);
            }
        }

        internal override MessageManager MessageManager => null;

        internal VoiceChannel(APIChannel model, Server server)
            : base(model, server)
        {
        }
        private VoiceChannel(ulong id, Server server)
            : base(id, server)
        {
        }


        /// <summary> Save all changes to this channel. </summary>
        public override async Task Save()
        {
            var request = new UpdateChannelRequest(Id)
            {
                Name = Name,
                Position = Position,
                Bitrate = Bitrate
            };
            await Client.ClientAPI.Send(request).ConfigureAwait(false);
        }

        internal override Channel Clone()
        {
            var result = new VoiceChannel(Id, Server);
            _cloner(this, result);
            return result;
        }
    }
}
