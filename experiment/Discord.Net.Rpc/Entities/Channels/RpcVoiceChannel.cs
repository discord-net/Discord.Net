using Discord.Audio;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Rpc.Channel;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcVoiceChannel : RpcGuildChannel, IRpcAudioChannel, IVoiceChannel
    {
        public int Bitrate { get; private set; }
        public int? UserLimit { get; private set; }
        public IReadOnlyCollection<RpcVoiceState> VoiceStates { get; private set; }

        internal RpcVoiceChannel(DiscordRpcClient discord, ulong id, ulong guildId)
            : base(discord, id, guildId)
        {
        }
        internal new static RpcVoiceChannel Create(DiscordRpcClient discord, Model model)
        {
            var entity = new RpcVoiceChannel(discord, model.Id, model.GuildId.Value);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);
            if (model.UserLimit.IsSpecified)
                UserLimit = model.UserLimit.Value != 0 ? model.UserLimit.Value : (int?)null;
            if (model.Bitrate.IsSpecified)
                Bitrate = model.Bitrate.Value;
            VoiceStates = model.VoiceStates.Select(x => RpcVoiceState.Create(Discord, x)).ToImmutableArray();
        }

        public Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);

        private string DebuggerDisplay => $"{Name} ({Id}, Voice)";

        //IAudioChannel
        Task<IAudioClient> IAudioChannel.ConnectAsync(Action<IAudioClient> configAction) { throw new NotSupportedException(); }
    }
}
