using Discord.Rest;

namespace Discord;

[
    Modifiable<ModifyGuildTemplateProperties>(nameof(Routes.ModifyGuildTemplate)),
    Deletable(nameof(Routes.DeleteGuildTemplate))
]
public partial interface IGuildTemplateFromGuildActor :
    IGuildTemplateActor,
    IGuildRelationship
{
    Task SyncAsync(
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.SyncGuildTemplate(Guild.Id, Id),
        options,
        token
    );
}