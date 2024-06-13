using Discord.Gateway.Cache;
using System;

namespace Discord.Gateway
{
    public sealed class GatewayStageChannel : GatewayVoiceChannel, IStageChannel, ICacheUpdatable<ulong, IStageInstanceModel>
    {
        public GuildStageInstanceCacheable? Instance { get; private set; }

        public GatewayStageChannel(DiscordGatewayClient discord, ulong guildId, IGuildVoiceChannelModel model)
            : base(discord, guildId, model)
        {

        }

        internal void Update(IStageInstanceModel model, CacheOperation operation)
        {
            Instance = EntityUtils.UpdateCacheableFrom(
                Discord,
                Instance,
                Discord.State.StageInstances,
                model.ChannelId,
                Guild.Id,
                operation,
                model.ChannelId == Id
            );
        }

        public Task BecomeSpeakerAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task ModifyInstanceAsync(Action<ModifyStageInstanceProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task MoveToSpeakerAsync(IGuildMember member, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveFromSpeakerAsync(IGuildMember member, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RequestToSpeakAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IStageInstance> StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, bool sendStartNotification = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopSpeakingAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopStageAsync(RequestOptions? options = null) => throw new NotImplementedException();

        void ICacheUpdatable<ulong, IStageInstanceModel>.Update(IStageInstanceModel model, CacheOperation operation)
            => Update(model, operation);

        ILoadableEntity<IStageInstance, ulong>? IStageChannel.StageInstance => Instance;
    }
}

