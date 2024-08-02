using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayMemberActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    MemberIdentity member,
    UserIdentity? user = null
) :
    GatewayCachedActor<ulong, GatewayMember, MemberIdentity, IMemberModel>(client, member),
    IGuildMemberActor,
    IGatewayCachedActor<ulong, GatewayMember, MemberIdentity, IMemberModel>
{
    [StoreRoot, SourceOfTruth] public GatewayGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth, ProxyInterface]
    public GatewayUserActor User { get; } =
        user?.Actor ?? new(client, user ?? member.Cast<GatewayUser, GatewayUserActor, IUserModel>());

    [SourceOfTruth] internal new MemberIdentity Identity { get; } = member;

    [SourceOfTruth]
    internal GatewayMember CreateEntity(IMemberModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayMember :
    GatewayCacheableEntity<GatewayMember, ulong, IMemberModel>,
    IGuildMember
{
    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public DateTimeOffset? JoinedAt => Model.JoinedAt;

    public string? Nickname => Model.Nickname;

    public string? GuildAvatarId => Model.Avatar;

    public DateTimeOffset? PremiumSince => Model.PremiumSince;

    public bool? IsPending => Model.IsPending;

    public DateTimeOffset? TimedOutUntil => Model.CommunicationsDisabledUntil;

    public GuildMemberFlags Flags => (GuildMemberFlags)Model.Flags;

    [ProxyInterface] internal GatewayMemberActor Actor { get; }

    internal IMemberModel Model { get; private set; }

    public GatewayMember(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IMemberModel model,
        UserIdentity? user = null,
        GatewayMemberActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, MemberIdentity.Of(this), user);
    }

    public static GatewayMember Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext context,
        IMemberModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        model,
        context.Path.GetIdentity(T<UserIdentity>(), model.Id),
        context.TryGetActor<GatewayMemberActor>()
    );

    public override ValueTask UpdateAsync(IMemberModel model, bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }


    public override IMemberModel GetModel() => Model;
}
