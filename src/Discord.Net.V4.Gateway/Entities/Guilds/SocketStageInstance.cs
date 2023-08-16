using Discord.Gateway.Cache;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway
{
    public sealed class SocketStageInstance : SocketCacheableEntity<ulong, IStageInstanceModel>, IStageInstance
    {
        public GuildCacheable Guild { get; }
        public GuildChannelCacheable Channel { get; }


        public ulong GuildId
            => _source.GuildId;

        public ulong ChannelId
            => _source.ChannelId;

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

        public SocketStageInstance(DiscordGatewayClient discord, IStageInstanceModel model)
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

        public Task ModifyAsync(Action<StageInstanceProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopAsync(RequestOptions? options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
    }
}

