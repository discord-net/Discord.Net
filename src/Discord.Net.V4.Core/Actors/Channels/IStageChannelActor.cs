using Discord.Models;
using Discord.Rest;
using Discord.Stage;

namespace Discord;

public interface ILoadableStageChannelActor<TChannel> :
    IStageChannelActor,
    ILoadableGuildChannelActor<TChannel>
    where TChannel : class, IStageChannel;

public interface IStageChannelActor :
    IGuildChannelActor,
    IStageInstanceRelationship,
    IActor<ulong, IStageChannel>
{
    async Task<IStageInstance> CreateStageInstanceAsync(
        string topic,
        StagePrivacyLevel? privacyLevel = null,
        bool? sendStartNotification = null,
        EntityOrId<ulong, IGuildScheduledEvent>? scheduledEvent = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateStageInstance(new CreateStageInstanceParams()
            {
                Topic = topic,
                ChannelId = Id,
                PrivacyLevel = Optional.FromNullable(privacyLevel).Map(v => (int)v),
                SendStartNotification = Optional.FromNullable(sendStartNotification),
                GuildScheduledEventId = Optional.FromNullable(scheduledEvent).Map(v => v.Id)
            }),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return Client.CreateEntity(result);
    }
}
