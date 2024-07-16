using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildStageChannelModel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IStageChannelActor :
    IVoiceChannelActor,
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
            Routes.CreateStageInstance(new CreateStageInstanceParams
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
