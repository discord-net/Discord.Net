using Discord.WebSocket.Cache;
using System;

namespace Discord.WebSocket
{
    public sealed class SocketStageChannel : SocketVoiceChannel, IStageChannel
    {
        public SocketStageChannel(DiscordSocketClient discord, ulong guildId, IGuildVoiceChannelModel model)
            : base(discord, guildId, model)
        {
        }

        public async ValueTask<SocketStageInstance?> GetStageInstance(CancellationToken token = default)
        {
            var guild = await Guild.GetAsync(token);

            if (guild is null)
                return null;

            return (await guild.StageInstances.FirstOrDefaultAwaitAsync(async cacheable =>
            {
                var entity = await cacheable.GetAsync(token);

                if (entity is null)
                    return false;

                return entity.EntityId == Id && entity.Type is GuildScheduledEventType.Stage;
            })).GetAsync(token);
        }

        public async ValueTask<IStageInstance?> GetOrFetchStageInstance(RequestOptions? options = null, CancellationToken token = default)
        {
            var guild = await Guild.GetOrFetchAsync(options, token);

            if (guild is null)
                return null;

            if(guild is SocketGuild sg)
            {
                return (await sg.StageInstances.FirstOrDefaultAwaitAsync(async cacheable =>
                {
                    var entity = await cacheable.GetOrFetchAsync(token);

                    if (entity is null)
                        return false;

                    return entity.EntityId == Id && entity.Type is GuildScheduledEventType.Stage;
                })).GetAsync(token);
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
    }
}

