using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    public class VoiceChannel : GuildChannel, IVoiceChannel
    {
        /// <inheritdoc />
        public int Bitrate { get; private set; }

        internal VoiceChannel(Guild guild, Model model)
            : base(guild, model)
        {
        }
        internal override void Update(Model model)
        {
            base.Update(model);
            Bitrate = model.Bitrate;
        }

        /// <inheritdoc />
        public async Task Modify(Action<ModifyVoiceChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyVoiceChannelParams();
            func(args);
            var model = await Discord.BaseClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model);
        }

        public override Task<GuildUser> GetUser(ulong id) { throw new NotSupportedException(); }
        public override Task<IEnumerable<GuildUser>> GetUsers() { throw new NotSupportedException(); }

        /// <inheritdoc />
        public override string ToString() => $"{base.ToString()} [Voice]";
    }
}
