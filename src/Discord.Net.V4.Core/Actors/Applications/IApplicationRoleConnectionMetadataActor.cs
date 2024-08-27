using Discord.Rest;

namespace Discord;

public interface IApplicationRoleConnectionMetadataActor :
    IActor<string, IApplicationRoleConnectionMetadata>,
    IApplicationRelationship
{
    [BackLink<IApplicationActor>]
    private static async Task<IEnumerable<IApplicationRoleConnectionMetadata>> UpdateAsync(
        ApplicationRoleConnectionMetadataLink.Enumerable.BackLink<IApplicationActor> link,
        IEnumerable<ModifyApplicationRoleConnectionMetadataProperties> metadatas,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await link.Client.RestApiClient.ExecuteRequiredAsync(
            Routes.ModifyApplicationRoleConnectionMetadata(
                link.Source.Id,
                metadatas.Select(x => x.ToApiModel())
            ),
            options,
            token
        );

        return result.Select(link.CreateEntity);
    }
}