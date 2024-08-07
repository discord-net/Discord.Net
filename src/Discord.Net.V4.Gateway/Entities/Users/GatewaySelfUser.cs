using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayCurrentUserActor :
    GatewayUserActor,
    ICurrentUserActor,
    IGatewayCachedActor<ulong, GatewayCurrentUser, SelfUserIdentity, ISelfUserModel>
{
    [SourceOfTruth] internal override SelfUserIdentity Identity { get; }

    public GatewayCurrentUserActor(
        DiscordGatewayClient client,
        SelfUserIdentity user
    ) : base(client, user)
    {
        Identity = user | this;
    }

    public IPartialGuild CreateEntity(IPartialGuildModel model) => throw new NotImplementedException();

    public IMember CreateEntity(IMemberModel model, ulong context) => throw new NotImplementedException();

    [SourceOfTruth]
    internal GatewayCurrentUser CreateEntity(ISelfUserModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayCurrentUser :
    GatewayUser,
    ICurrentUser,
    ICacheableEntity<GatewayCurrentUser, ulong, ISelfUserModel>
{
    public string Locale => Model.Locale!;

    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.PremiumType ?? PremiumType.None;

    [ProxyInterface] internal override GatewayCurrentUserActor Actor { get; }

    internal override ISelfUserModel Model => _model;

    private ISelfUserModel _model;

    public GatewayCurrentUser(
        DiscordGatewayClient client,
        ISelfUserModel model,
        GatewayCurrentUserActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, SelfUserIdentity.Of(this));
        _model = model;
    }

    public static GatewayCurrentUser Construct(DiscordGatewayClient client,
        IGatewayConstructionContext context, ISelfUserModel model)
    {
        return new GatewayCurrentUser(
            client,
            model,
            context.TryGetActor<GatewayCurrentUserActor>()
        );
    }

    [CovariantOverride]
    public ValueTask UpdateAsync(ISelfUserModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override ISelfUserModel GetModel() => Model;
}
