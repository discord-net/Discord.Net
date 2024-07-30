using Discord.Gateway.Cache;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway.Users;

public sealed partial class GatewaySelfUserActor(
    DiscordGatewayClient client,
    SelfUserIdentity identity
) :
    GatewayUserActor(client, identity),
    ISelfUserActor,
    IGatewayCachedActor<ulong, GatewaySelfUser, SelfUserIdentity, ISelfUserModel>
{
    public override SelfUserIdentity Identity { get; } = identity;

    public IPartialGuild CreateEntity(IPartialGuildModel model) => throw new NotImplementedException();

    public IGuildMember CreateEntity(IMemberModel model, ulong context) => throw new NotImplementedException();

    [SourceOfTruth]
    internal GatewaySelfUser CreateEntity(ISelfUserModel model)
        => Client.StateController.CreateLatent<ulong, GatewaySelfUser, ISelfUserModel>(this, model);

    [SourceOfTruth]
    private new async ValueTask<IEntityModelStore<ulong, ISelfUserModel>> GetStoreAsync(
        CancellationToken token = default
    ) => (await base.GetStoreAsync(token)).Cast(Template.Of<ISelfUserModel>());
}

public sealed partial class GatewaySelfUser :
    GatewayUser,
    ISelfUser,
    IStoreProvider<ulong, ISelfUserModel>,
    IBrokerProvider<ulong, GatewaySelfUser, ISelfUserModel>,
    ICacheableEntity<GatewaySelfUser, ulong, ISelfUserModel>,
    IContextConstructable<GatewaySelfUser, ISelfUserModel, ICacheConstructionContext<ulong, GatewaySelfUser>,
        DiscordGatewayClient>
{
    public string Locale => Model.Locale!;

    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.PremiumType ?? PremiumType.None;

    [ProxyInterface]
    internal override GatewaySelfUserActor Actor { get; }

    internal override ISelfUserModel Model => _model;

    private ISelfUserModel _model;

    public GatewaySelfUser(
        DiscordGatewayClient client,
        ISelfUserModel model,
        GatewaySelfUserActor? actor = null,
        IEntityHandle<ulong, GatewaySelfUser>? implicitHandle = null
    ) : base(client, model, actor, implicitHandle)
    {
        Actor = actor ??= new(client, SelfUserIdentity.Of(this));
        _model = model;
    }

    public static GatewaySelfUser Construct(DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewaySelfUser> context, ISelfUserModel model)
    {
        return new GatewaySelfUser(client, model, implicitHandle: context.ImplicitHandle);
    }

    [CovariantOverride]
    public ValueTask UpdateAsync(ISelfUserModel model, bool updateCache = true, CancellationToken token = default)
    {
        if(updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override ISelfUserModel GetModel() => Model;
}
