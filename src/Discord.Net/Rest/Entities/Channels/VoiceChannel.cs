using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
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
            var model = await Discord.ApiClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model);
        }

        protected override Task<GuildUser> GetUserInternal(ulong id) { throw new NotSupportedException(); }
        protected override Task<IEnumerable<GuildUser>> GetUsersInternal() { throw new NotSupportedException(); }
        protected override Task<IEnumerable<GuildUser>> GetUsersInternal(int limit, int offset) { throw new NotSupportedException(); }

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
    }
}
