using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetCurrentUser))]
[Modifiable<ModifySelfUserProperties>(nameof(Routes.ModifyCurrentUser))]
[BackLinkable]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface ICurrentUserActor :
    IUserActor,
    IActor<ulong, ICurrentUser>,
    IEntityProvider<IPartialGuild, IPartialGuildModel>,
    IEntityProvider<IMember, IMemberModel, ulong>
{
    // TODO:
    // - https://discord.com/developers/docs/resources/user#get-current-user-application-role-connection
    // - https://discord.com/developers/docs/resources/user#update-current-user-application-role-connection

    async Task<IReadOnlyCollection<UserConnection>> GetConnectionsAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var models = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetUserConnections,
            options ?? Client.DefaultRequestOptions,
            token
        );

        return models.Select(x => UserConnection.Construct(Client, x)).ToImmutableArray();
    }
}
