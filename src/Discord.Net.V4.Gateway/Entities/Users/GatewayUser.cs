using Discord.Gateway.Cache;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway.Users;

[ExtendInterfaceDefaults]
public partial class GatewayUserActor(
    DiscordGatewayClient client,
    UserIdentity user
) :
    GatewayCachedActor<ulong, GatewayUser, UserIdentity, IUserModel>(client, user),
    IUserActor
{
    [SourceOfTruth]
    internal GatewayUser CreateEntity(IUserModel model)
        => Client.StateController.CreateLatent<ulong, GatewayUser, IUserModel>(this, model);

    public IDMChannel CreateEntity(IDMChannelModel model) => throw new NotImplementedException();

    internal override ValueTask<IEntityModelStore<ulong, IUserModel>> GetStoreAsync(CancellationToken token = default)
        => Client.CacheProvider.GetStoreAsync<ulong, IUserModel>(token);
}

[ExtendInterfaceDefaults]
public partial class GatewayUser :
    GatewayCacheableEntity<GatewayUser, ulong, IUserModel, UserIdentity>,
    IStoreProvider<ulong, IUserModel>,
    IUser,
    IContextConstructable<GatewayUser, IUserModel, ICacheConstructionContext<ulong, GatewayUser>, DiscordGatewayClient>
{
    public string? AvatarId => Model.Avatar;
    public ushort Discriminator => ushort.Parse(Model.Discriminator);
    public string Username => Model.Username;
    public string? GlobalName => Model.GlobalName;
    public bool IsBot => Model.IsBot ?? false;
    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;

    [ProxyInterface(typeof(IUserActor), typeof(IStoreProvider<ulong, IUserModel>))]
    internal virtual GatewayUserActor Actor { get; }
    internal virtual IUserModel Model => _model;

    private IUserModel _model;

    internal GatewayUser(
        DiscordGatewayClient client,
        IUserModel model,
        GatewayUserActor? actor = null,
        IEntityHandle<ulong, GatewayUser>? implicitHandle = null
    ) : base(client, model.Id, implicitHandle)
    {
        Actor = actor ?? new(client, UserIdentity.Of(this));
        _model = model;
    }

    public static GatewayUser Construct(DiscordGatewayClient client, IUserModel model)
    {
        // switch for self user
        return new GatewayUser(client, model);
    }

    public static GatewayUser Construct(DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayUser> context, IUserModel model)
    {
        if (
            model is ISelfUserModel selfUser &&
            context is ICacheConstructionContext<ulong, GatewaySelfUser> selfUserContext)
        {
            return GatewaySelfUser.Construct(client, selfUserContext, selfUser);
        }

        return new GatewayUser(client, model, implicitHandle: context.ImplicitHandle);
    }

    public override ValueTask UpdateAsync(IUserModel model, bool updateCache = true, CancellationToken token = default)
    {
        // TODO: cache

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IUserModel GetModel() => Model;
}
