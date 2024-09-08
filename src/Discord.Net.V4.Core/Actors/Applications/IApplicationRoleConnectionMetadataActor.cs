using Discord.Rest;

namespace Discord;

public partial interface IApplicationRoleConnectionMetadataActor :
    IActor<string, IApplicationRoleConnectionMetadata>,
    IApplicationRelationship
{
    [BackLink<IApplicationActor>]
    private static async Task<IReadOnlyCollection<IApplicationRoleConnectionMetadata>> UpdateAsync(
        IApplicationActor application,
        ApplicationRoleConnectionMetadataLink link,
        IEnumerable<ModifyApplicationRoleConnectionMetadataProperties> metadatas,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await link.Client.RestApiClient.ExecuteRequiredAsync(
            Routes.ModifyApplicationRoleConnectionMetadata(
                application.Id,
                metadatas.Select(x => x.ToApiModel())
            ),
            options,
            token
        );

        return result
            .Select(link.CreateEntity)
            .ToList()
            .AsReadOnly();
    }
}