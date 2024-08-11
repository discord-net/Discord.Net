using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayCurrentMemberActor :
    GatewayMemberActor,
    IGatewayCachedActor<ulong, GatewayCurrentMember, CurrentMemberIdentity, IMemberModel>,
    ICurrentMemberActor
{
    [SourceOfTruth] public override GatewayCurrentUserActor User { get; }

    [SourceOfTruth] public override GatewayCurrentUserVoiceStateActor VoiceState { get; }

    [SourceOfTruth] internal override CurrentMemberIdentity Identity { get; }

    public GatewayCurrentMemberActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        CurrentMemberIdentity member,
        SelfUserIdentity? user = null
    ) : base(client, guild, member, user ?? client.CurrentUser.Identity)
    {
        Identity = member | this;

        User = Client.CurrentUser;
        VoiceState = new(client, guild | Guild, CurrentUserVoiceStateIdentity.Of(member.Id), Identity);
    }

    [SourceOfTruth]
    internal override GatewayCurrentMember CreateEntity(IMemberModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayCurrentMember :
    GatewayMember,
    ICacheableEntity<GatewayCurrentMember, ulong, IMemberModel>,
    ICurrentMember
{
    [ProxyInterface] internal override GatewayCurrentMemberActor Actor { get; }

    public GatewayCurrentMember(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IMemberModel model,
        GatewayCurrentMemberActor? actor = null,
        SelfUserIdentity? user = null
    ) : base(client, guild, model, actor, user)
    {
        Actor = actor ?? new(client, guild, CurrentMemberIdentity.Of(this), user);
    }

    public new static GatewayCurrentMember Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IMemberModel model
    ) => new(
        client,
        context.Path.RequireIdentity(Template.Of<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayCurrentMemberActor>(),
        context.Path.GetIdentity(Template.Of<SelfUserIdentity>())
    );

    public override ValueTask UpdateAsync(IMemberModel model, bool updateCache = true,
        CancellationToken token = default)
        => updateCache ? UpdateCacheAsync(this, model, token) : base.UpdateAsync(model, false, token);

    ValueTask IUpdatable<IMemberModel>.UpdateAsync(IMemberModel model, CancellationToken token)
        => UpdateAsync(model, true, token);
}
