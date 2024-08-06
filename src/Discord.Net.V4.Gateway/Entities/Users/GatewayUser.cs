using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayUserActor :
    GatewayCachedActor<ulong, GatewayUser, UserIdentity, IUserModel>,
    IUserActor
{
    internal override UserIdentity Identity { get; }

    [method: TypeFactory]
    public GatewayUserActor(
        DiscordGatewayClient client,
        UserIdentity user
    ) : base(client, user)
    {
        Identity = user | this;
    }

    [SourceOfTruth]
    internal GatewayUser CreateEntity(IUserModel model)
        => Client.StateController.CreateLatent(this, model);

    [SourceOfTruth]
    internal GatewayDMChannel CreateEntity(IDMChannelModel model)
        => Client.StateController.CreateLatent<ulong, GatewayDMChannel, GatewayDMChannelActor, IDMChannelModel>(
            model,
            CachePath
        );
}

[ExtendInterfaceDefaults]
public partial class GatewayUser :
    GatewayCacheableEntity<GatewayUser, ulong, IUserModel>,
    IUser
{
    public string? AvatarId => Model.Avatar;
    public ushort Discriminator => ushort.Parse(Model.Discriminator);
    public string Username => Model.Username;
    public string? GlobalName => Model.GlobalName;
    public bool IsBot => Model.IsBot ?? false;
    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;

    [ProxyInterface] internal virtual GatewayUserActor Actor { get; }
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

    public static GatewayUser Construct(DiscordGatewayClient client, IUserModel model)
    {
        // switch for self user
        return new GatewayUser(client, model);
    }

    public static GatewayUser Construct(DiscordGatewayClient client,
        IGatewayConstructionContext context, IUserModel model)
    {
        if (model is ISelfUserModel selfUser)
            return GatewaySelfUser.Construct(client, context, selfUser);

        return new GatewayUser(
            client,
            model,
            context.TryGetActor<GatewayUserActor>()
        );
    }

    public override ValueTask UpdateAsync(IUserModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IUserModel GetModel() => Model;
}
