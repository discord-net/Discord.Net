using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetGuildRole))]
[Deletable(nameof(Routes.DeleteGuildRole))]
[Creatable<CreateRoleProperties>(nameof(Routes.CreateGuildRole))]
[Modifiable<ModifyRoleProperties>(nameof(Routes.ModifyGuildRole))]
public partial interface IRoleActor :
    IGuildRelationship,
    IActor<ulong, IRole>
{
    [BackLink<IMemberActor>]
    private static async Task AddAsync(
        IMemberActor member, 
        EntityOrId<ulong, IRoleActor> role,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        await member.Client.RestApiClient.ExecuteAsync(
            Routes.AddGuildMemberRole(member.Guild.Id, member.Id, role.Id),
            options ?? member.Client.DefaultRequestOptions,
            token
        );
    }
    
    [BackLink<IMemberActor>]
    private static async Task RemoveAsync(
        IMemberActor member, 
        EntityOrId<ulong, IRoleActor> role,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        await member.Client.RestApiClient.ExecuteAsync(
            Routes.RemoveGuildMemberRole(member.Guild.Id, member.Id, role.Id),
            options ?? member.Client.DefaultRequestOptions,
            token
        );
    }
}
