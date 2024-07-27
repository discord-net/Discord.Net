namespace Discord.Gateway.Users;

[ExtendInterfaceDefaults]
public partial class GatewayUserActor(
    DiscordGatewayClient client,
    UserIdentity user
) :
    GatewayActor<ulong, GatewayUser, UserIdentity>(client, user),
    IUserActor
{
    [SourceOfTruth]
    internal GatewayUser CreateEntity(IUserModel model)
        => GatewayUser.Construct(Client, model);

    public IDMChannel CreateEntity(IDMChannelModel model) => throw new NotImplementedException();
}

[ExtendInterfaceDefaults]
public partial class GatewayUser :
    GatewayCacheableEntity<GatewayUser, ulong, IUserModel, UserIdentity>,
    IUser,
    IConstructable<GatewayUser, IUserModel, DiscordGatewayClient>,
    IContextConstructable<GatewayUser, IUserModel, IPathable, DiscordGatewayClient>
{
    public string? AvatarId => Model.Avatar;
    public ushort Discriminator => ushort.Parse(Model.Discriminator);
    public string Username => Model.Username;

    public string? GlobalName => Model.GlobalName;

    public bool IsBot => Model.IsBot ?? false;

    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;

    [ProxyInterface(typeof(IUserActor))]
    internal virtual GatewayUserActor Actor { get; }
    internal virtual IUserModel Model => _model;

    private IUserModel _model;

    internal GatewayUser(
        DiscordGatewayClient client,
        IUserModel model,
        GatewayUserActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, UserIdentity.Of(this));
        _model = model;
    }

    public static GatewayUser Construct(DiscordGatewayClient client, IPathable context, IUserModel model)
        => Construct(client, model);

    public static GatewayUser Construct(DiscordGatewayClient client, IUserModel model)
    {
        // switch for self user
        return new GatewayUser(client, model);
    }

    public override ValueTask UpdateAsync(IUserModel model, bool updateCache = true, CancellationToken token = default)
    {
        // TODO: cache

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IUserModel GetModel() => Model;
}
