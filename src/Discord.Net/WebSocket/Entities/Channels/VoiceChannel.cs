using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class VoiceChannel : GuildChannel, IVoiceChannel
    {
        /// <inheritdoc />
        public int Bitrate { get; private set; }
        /// <inheritdoc />
        public int UserLimit { get; private set; }

        public override IEnumerable<GuildUser> Users
            => Guild.Users.Where(x => x.VoiceChannel == this);

        internal VoiceChannel(Guild guild, Model model)
            : base(guild, model)
        {
        }
        internal override void Update(Model model)
        {
            base.Update(model);
            Bitrate = model.Bitrate;
            UserLimit = model.UserLimit;
        }

        /// <inheritdoc />
        public async Task Modify(Action<ModifyVoiceChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyVoiceChannelParams();
            func(args);
            await Discord.ApiClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
        }

        public override GuildUser GetUser(ulong id)
        {
            var member = _permissions.Get(id);
            if (member != null && member.Value.Permissions.ReadMessages)
                return member.Value.User;
            return null;
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
    }
}
