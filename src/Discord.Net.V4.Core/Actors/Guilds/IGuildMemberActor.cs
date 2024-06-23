using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildMemberActor :
    IGuildMemberActor,
    ILoadableEntity<ulong, IGuildMember>;

public interface IGuildMemberActor :
    IGuildRelationship,
    IUserRelationship,
    IModifiable<ulong, IGuildMemberActor, ModifyGuildUserProperties, ModifyGuildMemberParams>,
    IActor<ulong, IGuildMember>
{
    static IApiInRoute<ModifyGuildMemberParams>
        IModifiable<ulong, IGuildMemberActor, ModifyGuildUserProperties, ModifyGuildMemberParams>.ModifyRoute(
            IPathable path, ulong id,
            ModifyGuildMemberParams args)
        => Routes.ModifyGuildMember(path.Require<IGuild>(), id, args);

    Task AddRoleAsync(
        EntityOrId<ulong, IRole> role,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.AddGuildMemberRole(Guild.Id, Id, role.Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task RemoveRoleAsync(
        EntityOrId<ulong, IRole> role,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.RemoveGuildMemberRole(Guild.Id, Id, role.Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task KickAsync(
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.RemoveGuildMember(Guild.Id, Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task BanAsync(
        TimeSpan? pruneDuration = null,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.CreateGuildBan(
            Guild.Id,
            Id,
            new CreateGuildBanParams
            {
                DeleteMessageSeconds = Optional
                    .FromNullable(pruneDuration)
                    .Map(v => (int)Math.Floor(v.TotalSeconds))
            }
        ),
        options ?? Client.DefaultRequestOptions,
        token
    );

    Task UnbanAsync(
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.RemoveGuildBan(Guild.Id, Id),
        options ?? Client.DefaultRequestOptions,
        token
    );
}
