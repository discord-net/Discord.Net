using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public sealed partial class RestLoadableSelfUserActor(
    DiscordRestClient client,
    SelfUserIdentity user
) :
    RestSelfUserActor(client, user),
    ILoadableSelfUserActor
{
    [ProxyInterface(typeof(ILoadableEntity<ISelfUser>))]
    internal RestLoadable<ulong, RestSelfUser, ISelfUser, ISelfUserModel> Loadable { get; } =
        RestLoadable<ulong, RestSelfUser, ISelfUser, ISelfUserModel>.FromConstructable<RestSelfUser>(
            client,
            user,
            Routes.GetCurrentUser
        );
}

[ExtendInterfaceDefaults(
    typeof(ISelfUserActor),
    typeof(IModifiable<ulong, ISelfUserActor, ModifySelfUserProperties, ModifyCurrentUserParams>)
)]
public partial class RestSelfUserActor(
    DiscordRestClient client,
    SelfUserIdentity user
) :
    RestUserActor(client, user),
    ISelfUserActor
{
    ISelfUser IEntityProvider<ISelfUser, ISelfUserModel>.CreateEntity(ISelfUserModel model)
        => RestSelfUser.Construct(Client, model);
}

public partial class RestSelfUser :
    RestUser,
    ISelfUser,
    IConstructable<RestSelfUser, ISelfUserModel, DiscordRestClient>
{
    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.PremiumType ?? PremiumType.None;

    public string Locale => Model.Locale!;

    [ProxyInterface(
        typeof(ISelfUserActor),
        typeof(IEntityProvider<ISelfUser, ISelfUserModel>)
    )]
    internal override RestSelfUserActor Actor { get; }

    internal override ISelfUserModel Model => _model;

    private ISelfUserModel _model;

    internal RestSelfUser(
        DiscordRestClient client,
        ISelfUserModel model,
        RestSelfUserActor? actor = null
    ) : base(client, model)
    {
        Actor = actor ?? new(client, SelfUserIdentity.Of(this));
        _model = model;
    }

    public static RestSelfUser Construct(DiscordRestClient client, ISelfUserModel model) => new(client, model);

    public ValueTask UpdateAsync(ISelfUserModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override ISelfUserModel GetModel() => Model;
}
