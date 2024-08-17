using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public partial class GatewayMemberActor :
    GatewayCachedActor<ulong, GatewayMember, MemberIdentity, IMemberModel>,
    IMemberActor
{
    [StoreRoot, SourceOfTruth] public GatewayGuildActor Guild { get; }

    [SourceOfTruth, ProxyInterface] public virtual GatewayUserActor User { get; }

    [SourceOfTruth]
    public virtual GatewayVoiceStateActor VoiceState { get; }

    internal override MemberIdentity Identity { get; }

    [method: TypeFactory(LastParameter = nameof(member))]
    public GatewayMemberActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        MemberIdentity member,
        UserIdentity? user = null
    ) : base(client, member)
    {
        member = Identity = member | this;

        Guild = client.Guilds >> guild;
        User = client.Users >> (user | member);
        VoiceState = new(client, guild | Guild, VoiceStateIdentity.Of(member.Id), member);
    }

    [SourceOfTruth]
    internal virtual GatewayMember CreateEntity(IMemberModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public partial class GatewayMember :
    GatewayCacheableEntity<GatewayMember, ulong, IMemberModel>,
    IMember
{
    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public DateTimeOffset? JoinedAt => Model.JoinedAt;

    public string? Nickname => Model.Nickname;

    public string? GuildAvatarId => Model.Avatar;

    public DateTimeOffset? PremiumSince => Model.PremiumSince;

    public bool? IsPending => Model.IsPending;

    public DateTimeOffset? TimedOutUntil => Model.CommunicationsDisabledUntil;

    public GuildMemberFlags Flags => (GuildMemberFlags)Model.Flags;

    [ProxyInterface] internal virtual GatewayMemberActor Actor { get; }

    internal IMemberModel Model { get; private set; }

    public GatewayMember(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IMemberModel model,
        GatewayMemberActor? actor = null,
        UserIdentity? user = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, MemberIdentity.Of(this), user);
    }

    public static GatewayMember Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IMemberModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayMemberActor>(),
        context.Path.GetIdentity(T<UserIdentity>(), model.Id)
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
