using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord;

[Loadable(nameof(Routes.GetGuild))]
[Modifiable<ModifyGuildProperties>(nameof(Routes.ModifyGuild))]
[Deletable(nameof(Routes.DeleteGuild))]
public partial interface IGuildActor :
    IActor<ulong, IGuild>,
    IHasThreadsTrait<
        IThreadChannelActor, 
        IThreadChannelActor.Indexable.WithActive.BackLink<IGuildActor>>,
    IInvitableTrait<IGuildInviteActor, IGuildInvite>
{
    [return: TypeHeuristic(nameof(Sounds))]
    IGuildSoundboardSoundActor Sound(ulong id) => Sounds[id];
    IGuildSoundboardSoundActor.Enumerable.Indexable Sounds { get; }
    
    [return: TypeHeuristic(nameof(Templates))]
    IGuildTemplateFromGuildActor Template(string code) => Templates[code];
    IGuildTemplateFromGuildActor.Enumerable.Indexable.BackLink<IGuildActor> Templates { get; }
    
    [return: TypeHeuristic(nameof(Channels))]
    IGuildChannelActor Channel(ulong id) => Channels[id];
    IGuildChannelActor.Enumerable.Indexable.Hierarchy.BackLink<IGuildActor> Channels { get; }

    [return: TypeHeuristic(nameof(Integrations))]
    IIntegrationActor Integration(ulong id) => Integrations[id];
    IIntegrationActor.Enumerable.Indexable Integrations { get; }

    [return: TypeHeuristic(nameof(Bans))]
    IBanActor Ban(ulong userId) => Bans[userId];
    IBanActor.Paged<PageGuildBansParams>.Indexable.BackLink<IGuildActor> Bans { get; }

    [return: TypeHeuristic(nameof(Members))]
    IMemberActor Member(ulong id) => Members[id];
    IMemberActor.Paged<PageGuildMembersParams>.Indexable.WithCurrent Members { get; }

    [return: TypeHeuristic(nameof(Emotes))]
    IGuildEmoteActor Emote(ulong id) => Emotes[id];
    IGuildEmoteActor.Enumerable.Indexable.BackLink<IGuildActor> Emotes { get; }

    [return: TypeHeuristic(nameof(Roles))]
    IRoleActor Role(ulong id) => Roles[id];
    IRoleActor.Enumerable.Indexable.BackLink<IGuildActor> Roles { get; }

    [return: TypeHeuristic(nameof(Stickers))]
    IGuildStickerActor Sticker(ulong id) => Stickers[id];
    IGuildStickerActor.Enumerable.Indexable.BackLink<IGuildActor> Stickers { get; }

    [return: TypeHeuristic(nameof(ScheduledEvents))]
    IGuildScheduledEventActor ScheduledEvent(ulong id) => ScheduledEvents[id];
    IGuildScheduledEventActor.Enumerable.Indexable.BackLink<IGuildActor> ScheduledEvents { get; }

    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
    IWebhookActor.Enumerable.Indexable Webhooks { get; }

    #region Methods

    Task LeaveAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {   
        return Client.RestApiClient.ExecuteAsync(
            Routes.LeaveGuild(Id),
            options ?? Client.DefaultRequestOptions,
            token
        );
    }
    
    async Task<MfaLevel> ModifyMFALevelAsync(
        MfaLevel level,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.ModifyGuildMfaLevel(Id, new ModifyGuildMfaLevelParams {Level = (int)level}),
            options ?? Client.DefaultRequestOptions,
            token
        );
        
        return (MfaLevel)result.Level;
    }

    async Task<int> GetPruneCountAsync(
        int? days = null,
        Optional<IEnumerable<EntityOrId<ulong, IRole>>> includeRoles = default,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetGuildPruneCount(
                Id,
                days,
                ~includeRoles.Map(v => v.Select(v => v.Id).ToArray())
            ),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result.Pruned;
    }

    async Task<int?> BeginPruneAsync(
        int? days = null,
        bool? computePruneCount = null,
        Optional<IEnumerable<EntityOrId<ulong, IRole>>> includeRoles = default,
        string? reason = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteAsync(
            Routes.BeginGuildPrune(Id,
                new BeginGuildPruneParams
                {
                    Days = Optional.FromNullable(days),
                    Reason = Optional.FromNullable(reason),
                    ComputePruneCount = Optional.FromNullable(computePruneCount),
                    IncludeRoleIds = includeRoles.Map(v => v.Select(v => v.Id).ToArray())
                }),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result?.Pruned;
    }

    async Task<IReadOnlyCollection<VoiceRegion>> GetGuildVoiceRegionsAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetGuildVoiceRegions(Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result.Select(v => VoiceRegion.Construct(Client, v)).ToImmutableArray();
    }

    #endregion

    [LinkExtension]
    private interface WithTemplatesExtension
    {
        IGuildTemplateActor.Indexable Templates { get; }
    }
}
