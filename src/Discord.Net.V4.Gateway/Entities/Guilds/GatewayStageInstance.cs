using Discord.Gateway.Cache;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway
{
    public sealed class GatewayStageInstance : GatewayCacheableEntity<ulong, IStageInstanceModel>, IStageInstance
    {
        public GuildCacheable Guild { get; }

        public GuildChannelCacheable Channel { get; }

        public string Topic
            => _source.Topic;

        public StagePrivacyLevel PrivacyLevel
            => _source.PrivacyLevel;

        public ulong? ScheduledEventId
            => _source.EventId;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        protected override IStageInstanceModel Model
            => _source;

        private IStageInstanceModel _source;

        public GatewayStageInstance(DiscordGatewayClient discord, IStageInstanceModel model)
            : base(discord, model.Id)
        {
            Update(model);

            Guild = new(model.GuildId, discord, discord.State.Guilds.ProvideSpecific(model.GuildId));
            Channel = new(model.ChannelId, discord, discord.State.GuildChannels.ProvideSpecific(model.ChannelId));
        }


        [MemberNotNull(nameof(_source))]
        internal override void Update(IStageInstanceModel model)
        {
            _source = model;
        }

        public Task ModifyAsync(Action<ModifyStageInstanceProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopAsync(RequestOptions? options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        IEntitySource<IGuild, ulong> IStageInstance.Guild => Guild;

        IEntitySource<IGuildChannel, ulong> IStageInstance.Channel => Channel;
    }
}

