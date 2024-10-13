using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildMember)),
    Modifiable<ModifyGuildUserProperties>(nameof(Routes.ModifyGuildMember)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IMemberActor :
    IUserActor,
    IGuildActor.CanonicalRelationship,
    IActor<ulong, IMember>
{
    IVoiceStateActor VoiceState { get; }
    IRoleActor.Indexable.BackLink<IMemberActor> Roles { get; }

    [BackLink<IGuildActor>]
    private static async Task<IMember> AddAsync(
        IGuildActor guild,
        MemberLink link,
        EntityOrId<ulong, IUser> user,
        string accessToken,
        string? nickname = null,
        IEnumerable<EntityOrId<ulong, IRole>>? roles = null,
        bool? mute = null,
        bool? deaf = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return link.CreateEntity(
            await link.Client.RestApiClient.ExecuteRequiredAsync(
                Routes.AddGuildMember(
                    guild.Id,
                    user.Id,
                    new AddGuildMemberParams
                    {
                        AccessToken = accessToken,
                        Nickname = Optional.FromNullable(nickname),
                        IsDeaf = Optional.FromNullable(deaf),
                        IsMute = Optional.FromNullable(mute),
                        RoleIds = Optional.FromNullable(roles)
                            .Map(v => v
                                .Select(v => v.Id)
                                .ToArray()
                            )
                    }
                ),
                options,
                token
            )
        );
    }

    [BackLink<IGuildActor>]
    private static async Task<IReadOnlyCollection<IMember>> SearchAsync(
        IGuildActor guild,
        MemberLink link,
        string query,
        int limit = DiscordConfig.MaxUsersPerBatch,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await link.Client.RestApiClient.ExecuteRequiredAsync(
            Routes.SearchGuildMembers(
                guild.Id,
                query,
                limit
            ),
            options,
            token
        );

        return result.Select(link.CreateEntity).ToList().AsReadOnly();
    }

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
                    .Map(v => (int) Math.Floor(v.TotalSeconds))
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

    [LinkExtension]
    private interface WithCurrentExtension
    {
        ICurrentMemberActor Current { get; }
    }
}