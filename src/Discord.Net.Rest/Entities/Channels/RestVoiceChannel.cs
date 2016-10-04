using Discord.API.Rest;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestVoiceChannel : RestGuildChannel, IVoiceChannel, IRestAudioChannel
    {
        public int Bitrate { get; private set; }
        public int UserLimit { get; private set; }

        internal RestVoiceChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, guild, id)
        {
        }
        internal new static RestVoiceChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestVoiceChannel(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Bitrate = model.Bitrate.Value;
            UserLimit = model.UserLimit.Value;
        }

        public Task ModifyAsync(Action<ModifyVoiceChannelParams> func)
            => ChannelHelper.ModifyAsync(this, Discord, func);

        //IVoiceChannel
        Task<IAudioClient> IVoiceChannel.ConnectAsync() { throw new NotSupportedException(); }

        //IGuildChannel
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IGuildUser>(null);
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>().ToAsyncEnumerable();
    }
}
