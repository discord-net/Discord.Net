using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    public class VoiceChannel : GuildChannel
    {
        /// <inheritdoc />
        public int Bitrate { get; private set; }

        /// <inheritdoc />
        public override ChannelType Type => ChannelType.Voice;
                
        internal VoiceChannel(ulong id, Guild guild, bool usePermissionsCache)
            : base(id, guild, usePermissionsCache)
        {
        }
        
        internal override void Update(Model model)
        {
            Bitrate = model.Bitrate;
            base.Update(model);
        }

        public async Task Modify(Action<ModifyVoiceChannelRequest> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var req = new ModifyVoiceChannelRequest(Id);
            func(req);
            await Discord.RestClient.Send(req).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override string ToString() => $"{Guild}/{Name ?? Id.ToString()}";
    }
}
