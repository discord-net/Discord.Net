using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableGuildMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    MemberIdentity member
):
    RestGuildMemberActor(client, guild, member),
    ILoadableGuildMemberActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildMember>))]
    internal RestLoadable<ulong, RestGuildMember, IGuildMember, IMemberModel> Loadable { get; } =
        RestLoadable<ulong, RestGuildMember, IGuildMember, IMemberModel>
            .FromContextConstructable<RestGuildMember, GuildIdentity>(
                client,
                member,
                (guild, id) => Routes.GetGuildMember(guild.Id, id),
                guild
            );
}

[ExtendInterfaceDefaults(typeof(IGuildMemberActor))]
public partial class RestGuildMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    MemberIdentity member,
    UserIdentity? user = null
):
    RestUserActor(client, UserIdentity.Of(member.Id)),
    IGuildMemberActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);
    public RestLoadableUserActor User { get; } = new(client, user ?? UserIdentity.Of(member.Id));

    ILoadableGuildActor IGuildRelationship.Guild => Guild;

    ILoadableUserActor IUserRelationship.User => User;

    IGuildMember IEntityProvider<IGuildMember, IMemberModel>.CreateEntity(IMemberModel model)
        => RestGuildMember.Construct(Client, model, Guild.Identity);
}

public sealed partial class RestGuildMember :
    RestEntity<ulong>,
    IGuildMember,
    IContextConstructable<RestGuildMember, IMemberModel, GuildIdentity, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public DateTimeOffset? JoinedAt => Model.JoinedAt;

    public string? Nickname => Model.Nickname;

    public string? GuildAvatarId => Model.Avatar;

    public DateTimeOffset? PremiumSince => Model.PremiumSince;

    public bool? IsPending => Model.IsPending;

    public DateTimeOffset? TimedOutUntil => Model.CommunicationsDisabledUntil;

    public GuildMemberFlags Flags => (GuildMemberFlags)Model.Flags;

    [ProxyInterface(
        typeof(IGuildMemberActor),
        typeof(IUserRelationship),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildMember, IMemberModel>)
    )]
    internal RestGuildMemberActor Actor { get; }

    internal IMemberModel Model { get; private set; }

    internal RestGuildMember(
        DiscordRestClient client,
        GuildIdentity guild,
        IMemberModel model,
        RestGuildMemberActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(
            client,
            guild,
            MemberIdentity.Of(this),
            UserIdentity.OfNullable(
                model.GetReferencedEntityModel<ulong, IUserModel>(model.Id),
                model => RestUser.Construct(client, model)
            )
        );
        Model = model;
    }

    public static RestGuildMember Construct(DiscordRestClient client, IMemberModel model, GuildIdentity guild)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IMemberModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IMemberModel GetModel() => Model;
}
