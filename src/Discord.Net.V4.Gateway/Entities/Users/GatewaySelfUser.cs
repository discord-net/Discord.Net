using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewaySelfUserActor :
    GatewayUserActor,
    ISelfUserActor,
    IGatewayCachedActor<ulong, GatewaySelfUser, SelfUserIdentity, ISelfUserModel>
{
    [SourceOfTruth] internal override SelfUserIdentity Identity { get; }

    public GatewaySelfUserActor(
        DiscordGatewayClient client,
        SelfUserIdentity user
    ) : base(client, user)
    {
        Identity = user | this;
    }

    public IPartialGuild CreateEntity(IPartialGuildModel model) => throw new NotImplementedException();

    public IGuildMember CreateEntity(IMemberModel model, ulong context) => throw new NotImplementedException();

    [SourceOfTruth]
    internal GatewaySelfUser CreateEntity(ISelfUserModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewaySelfUser :
    GatewayUser,
    ISelfUser,
    ICacheableEntity<GatewaySelfUser, ulong, ISelfUserModel>
{
    public string Locale => Model.Locale!;

    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.PremiumType ?? PremiumType.None;

    [ProxyInterface] internal override GatewaySelfUserActor Actor { get; }

    internal override ISelfUserModel Model => _model;

    private ISelfUserModel _model;

    public GatewaySelfUser(
        DiscordGatewayClient client,
        ISelfUserModel model,
        GatewaySelfUserActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, SelfUserIdentity.Of(this));
        _model = model;
    }

    public static GatewaySelfUser Construct(DiscordGatewayClient client,
        IGatewayConstructionContext context, ISelfUserModel model)
    {
        return new GatewaySelfUser(
            client,
            model,
            context.TryGetActor<GatewaySelfUserActor>()
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
