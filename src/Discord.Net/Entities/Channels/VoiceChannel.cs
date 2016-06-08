using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class VoiceChannel : GuildChannel, IVoiceChannel
    {
        public int Bitrate { get; private set; }
        public int UserLimit { get; private set; }

        public VoiceChannel(Guild guild, Model model)
            : base(guild, model)
        {
        }
        protected override void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            base.Update(model, UpdateSource.Rest);
            Bitrate = model.Bitrate;
            UserLimit = model.UserLimit;
        }
        
        public async Task Modify(Action<ModifyVoiceChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyVoiceChannelParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }

        public override Task<IGuildUser> GetUser(ulong id)
        {
            throw new NotSupportedException();
        }
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers()
        {
            throw new NotSupportedException();
        }
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset)
        {
            throw new NotSupportedException();
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";
    }
}
