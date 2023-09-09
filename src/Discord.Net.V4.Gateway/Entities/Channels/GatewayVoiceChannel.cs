using Discord.Audio;
using Discord.Gateway.Cache;
using System;

namespace Discord.Gateway
{
    public class GatewayVoiceChannel : GatewayTextChannel, IVoiceChannel, IGatewayNestedChannel
    {
        public int Bitrate
            => _source.Bitrate;

        public int? UserLimit
            => _source.UserLimit;

        public VideoQualityMode VideoQualityMode
            => _source.VideoQualityMode;

        public string? RTCRegion
            => _source.RTCRegion;

        protected override IGuildVoiceChannelModel Model
            => _source;

        private IGuildVoiceChannelModel _source;

        public GatewayVoiceChannel(DiscordGatewayClient discord, ulong guildId, IGuildVoiceChannelModel model)
            : base(discord, guildId, model)
        {
            _source = model;
        }

        internal void Update(IGuildVoiceChannelModel model)
        {
            _source = model;
            base.Update(model);
        }

        public Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false) => throw new NotImplementedException();
        public Task DisconnectAsync() => throw new NotImplementedException();
        public Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task ModifyAsync(Action<ModifyAudioChannelProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
    }
}

