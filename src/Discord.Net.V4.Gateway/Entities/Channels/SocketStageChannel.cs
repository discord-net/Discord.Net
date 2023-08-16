using Discord.Gateway.Cache;
using System;

namespace Discord.Gateway
{
    public sealed class SocketStageChannel : SocketVoiceChannel, IStageChannel
    {
        public IStageInstance StageInstance => throw new NotImplementedException();

        public SocketStageChannel(DiscordGatewayClient discord, ulong guildId, IGuildVoiceChannelModel model)
            : base(discord, guildId, model)
        {
        }

        public async ValueTask<SocketStageInstance?> GetStageInstance(CancellationToken token = default)
        {
            var guild = await Guild.GetAsync(token);

            if (guild is null)
                return null;

            return (await guild.StageInstances.FlattenAsync(token)).FirstOrDefault(x => x?.ChannelId == Id);
        }

        public async ValueTask<IStageInstance?> GetOrFetchStageInstance(RequestOptions? options = null, CancellationToken token = default)
        {
            var cachedInstance = await GetStageInstance(token);

            if (cachedInstance is not null)
                return cachedInstance;

            // TODO: REST impl
            return null;
        }

        public Task BecomeSpeakerAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task MoveToSpeakerAsync(IGuildUser user, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveFromSpeakerAsync(IGuildUser user, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RequestToSpeakAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task<IStageInstance> StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, bool sendStartNotification = false, RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopSpeakingAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task StopStageAsync(RequestOptions? options = null) => throw new NotImplementedException();
    }
}

