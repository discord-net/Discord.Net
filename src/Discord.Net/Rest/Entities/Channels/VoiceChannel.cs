using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override async Task<IEnumerable<GuildUser>> GetUsers()
        {
            var users = await Guild.GetUsers().ConfigureAwait(false);
            return users.Where(x => PermissionUtilities.GetValue(PermissionHelper.Resolve(x, this), ChannelPermission.Connect));
        }

        /// <inheritdoc />
        public override string ToString() => $"{base.ToString()} [Voice]";
    }
}
