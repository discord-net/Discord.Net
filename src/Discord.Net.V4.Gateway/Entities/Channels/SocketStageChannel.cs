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
            // new model channel id isn't ours
            if (Instance is not null && operation is CacheOperation.Update && model.ChannelId != Id)
            {
                Instance = null;
                return;
            }

            // the model isn't for us
            if (model.ChannelId != Id)
                return;

            // if our instance cacheable is null and the models channel id points to us.
            if (Instance is null && operation is CacheOperation.Create or CacheOperation.Update)
            {
                Instance = new GuildStageInstanceCacheable(model.Id, Guild.Id, Discord, Discord.State.StageInstances.ProvideSpecific(model.Id, Guild.Id));
            }
            // if the operation was a delete and we still hold the instance
            else if (operation is CacheOperation.Delete && Instance is not null)
            {
                Instance = null;
            }
        }

        public Task BecomeSpeakerAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task MoveToSpeakerAsync(IGuildUser user, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveFromSpeakerAsync(IGuildUser user, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RequestToSpeakAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IStageInstance> StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, bool sendStartNotification = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopSpeakingAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopStageAsync(RequestOptions? options = null) => throw new NotImplementedException();

        void ICacheUpdatable<ulong, IStageInstanceModel>.Update(IStageInstanceModel model, CacheOperation operation)
            => Update(model, operation);

        IEntitySource<IStageInstance, ulong>? IStageChannel.StageInstance => throw new NotImplementedException();
    }
}

