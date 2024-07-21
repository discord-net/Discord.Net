using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;

[method: TypeFactory(LastParameter = nameof(member))]
[ExtendInterfaceDefaults]
public partial class RestGuildMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    MemberIdentity member,
    UserIdentity? user = null
):
    RestActor<ulong, RestGuildMember, MemberIdentity>(client, member),
    IGuildMemberActor
{
    public override MemberIdentity Identity { get; } = member;

    [SourceOfTruth]
    public RestGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth]
    [ProxyInterface(typeof(IUserActor))]
    public RestUserActor User { get; } = user?.Actor ?? new(client, user ?? UserIdentity.Of(member.Id));

    [SourceOfTruth]
    internal RestGuildMember CreateEntity(IMemberModel model)
        => RestGuildMember.Construct(Client, Guild.Identity, model);
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

    [ProxyInterface(typeof(IGuildMemberActor))]
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
            UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, model.Id, client)
        );
        Model = model;
    }

    public static RestGuildMember Construct(DiscordRestClient client, GuildIdentity guild, IMemberModel model)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IMemberModel model, CancellationToken token = default)
    {
        Model = model;
        return ValueTask.CompletedTask;
    }

    public IMemberModel GetModel() => Model;
}
