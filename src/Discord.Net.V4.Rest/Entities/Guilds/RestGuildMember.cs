using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableGuildMemberActor(DiscordRestClient client, ulong guildId, ulong id, IMemberModel? model = null) :
    RestGuildMemberActor(client, guildId, id),
    ILoadableGuildMemberActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildMember>))]
    internal RestLoadable<ulong, RestGuildMember, IGuildMember, IMemberModel> Loadable { get; } =
        RestLoadable<ulong, RestGuildMember, IGuildMember, IMemberModel>
            .FromContextConstructable<RestGuildMember, RestGuildMember.ConstructionContext>(
                client,
                id,
                (ctx, id) => Routes.GetGuildMember(ctx.GuildId, id),
                new(guildId, id),
                model
            );
}

[ExtendInterfaceDefaults(typeof(IGuildMemberActor))]
public partial class RestGuildMemberActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestActor<ulong, RestGuildMember>(client, id),
    IGuildMemberActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);
    public RestLoadableUserActor User { get; } = new(client, id);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;

    ILoadableUserActor IUserRelationship.User => User;
}

public sealed partial class RestGuildMember(DiscordRestClient client, ulong guildId, ulong userId, IMemberModel model, RestGuildMemberActor? actor = null) :
    RestEntity<ulong>(client, userId),
    IGuildMember,
    IContextConstructable<RestGuildMember, IMemberModel, RestGuildMember.ConstructionContext, DiscordRestClient>
{
    public readonly record struct ConstructionContext(ulong GuildId, ulong UserId);

    [ProxyInterface(
        typeof(IGuildMemberActor),
        typeof(IUserRelationship),
        typeof(IGuildRelationship)
    )]
    internal RestGuildMemberActor Actor { get; } = actor ?? new(client, guildId, userId);

    internal IMemberModel Model { get; } = model;

    public static RestGuildMember Construct(DiscordRestClient client, IMemberModel model, ConstructionContext context)
        => new(client, context.GuildId, context.UserId, model);

    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public DateTimeOffset? JoinedAt => Model.JoinedAt;

    public string? Nickname => Model.Nickname;

    public string? GuildAvatarId => Model.Avatar;

    public DateTimeOffset? PremiumSince => Model.PremiumSince;

    public bool? IsPending => Model.IsPending;

    public DateTimeOffset? TimedOutUntil => Model.CommunicationsDisabledUntil;

    public GuildMemberFlags Flags => (GuildMemberFlags)Model.Flags;

}
