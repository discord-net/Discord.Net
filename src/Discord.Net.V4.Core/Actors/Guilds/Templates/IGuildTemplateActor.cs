using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildTemplate)),
    Creatable<CreateGuildTemplateProperties>(
        nameof(Routes.CreateGuildTemplate),
        nameof(IGuildActor)
    )
]
public partial interface IGuildTemplateActor :
    IActor<string, IGuildTemplate>
{
    async Task<IGuild> CreateGuildAsync(
        CreateGuildFromTemplateProperties args,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateGuildFromTemplate(
                Id,
                args.ToApiModel()
            ),
            options,
            token
        );

        return Client.Guilds.CreateEntity(model);
    }
}