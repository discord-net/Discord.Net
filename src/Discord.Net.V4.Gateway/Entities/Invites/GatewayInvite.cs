using Discord.Models;

namespace Discord.Gateway;

public partial class GatewayInviteActor :
    GatewayCachedActor<string, GatewayInvite, InviteIdentity, IInviteModel>,
    IInviteActor
{
    internal override InviteIdentity Identity { get; }

    public GatewayInviteActor(
        DiscordGatewayClient client,
        InviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;
    }

    [SourceOfTruth]
    internal virtual GatewayInvite CreateEntity(IInviteModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public partial class GatewayInvite :
    GatewayCacheableEntity<GatewayInvite, string, IInviteModel>,
    IInvite
{
    public InviteType Type => (InviteType)Model.Type;

    [SourceOfTruth]
    public GatewayUserActor? Inviter { get; private set; }

    public InviteTargetType? TargetType => (InviteTargetType?)Model.TargetType;

    [SourceOfTruth]
    public GatewayUserActor? TargetUser { get; private set; }

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => Model.ExpiresAt;

    [ProxyInterface] internal virtual GatewayInviteActor Actor { get; }

    internal IInviteModel Model { get; private set; }

    internal GatewayInvite(
        DiscordGatewayClient client,
        IInviteModel model,
        GatewayInviteActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, InviteIdentity.Of(this));

        UpdateLinkedActors(model);
    }

    public static GatewayInvite Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IInviteModel model)
    {
        // TODO: switch invite guild/channel

        return new GatewayInvite(
            client,
            model,
            context.TryGetActor<GatewayInviteActor>()
        );
    }

    private void UpdateLinkedActors(IInviteModel model)
    {
        Inviter = Inviter.UpdateFrom(
            model.InviterId,
            (client, user) => client.Users >> user,
            Client
        );

        TargetUser = TargetUser.UpdateFrom(
            model.TargetUserId,
            (client, user) => client.Users >> user,
            Client
        );
    }

    public override ValueTask UpdateAsync(
        IInviteModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        UpdateLinkedActors(model);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IInviteModel GetModel() => Model;
}
