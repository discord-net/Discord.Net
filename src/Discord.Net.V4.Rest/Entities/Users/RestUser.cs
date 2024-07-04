using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class RestLoadableUserActor(
    DiscordRestClient client,
    UserIdentity user
):
    RestUserActor(client, user),
    ILoadableUserActor
{
    [ProxyInterface(typeof(ILoadableEntity<IUser>))]
    internal RestLoadable<ulong, RestUser, IUser, IUserModel> Loadable { get; } =
        RestLoadable<ulong, RestUser, IUser, IUserModel>.FromConstructable<RestUser>(
            client,
            user,
            Routes.GetUser
        );
}

[ExtendInterfaceDefaults(typeof(IUserActor))]
public partial class RestUserActor(
    DiscordRestClient client,
    UserIdentity user
) :
    RestActor<ulong, RestSelfUser>(client, user.Id),
    IUserActor;

public partial class RestUser :
    RestEntity<ulong>,
    IUser,
    IConstructable<RestUser, IUserModel, DiscordRestClient>
{
    public string? AvatarId => Model.Avatar;

    public ushort Discriminator => Model.Discriminator;

    public string Username => Model.Username;

    public string? GlobalName => Model.GlobalName;

    public bool IsBot => Model.IsBot ?? false;

    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;

    [ProxyInterface(typeof(IUserActor))]
    internal virtual RestUserActor Actor { get; }

    internal virtual IUserModel Model => _model;

    private IUserModel _model;

    internal RestUser(DiscordRestClient client, IUserModel model, RestUserActor? actor = null) : base(client, model.Id)
    {
        Actor = actor ?? new(client, UserIdentity.Of(this));
        _model = model;
    }

    public static RestUser Construct(DiscordRestClient client, IUserModel model)
    {
        return model switch
        {
            ISelfUserModel selfUserModel => RestSelfUser.Construct(client, selfUserModel),
            _ => new RestUser(client, model)
        };
    }

    public ValueTask UpdateAsync(IUserModel model, CancellationToken token = default)
    {
        _model = model;

        return ValueTask.CompletedTask;
    }

    public virtual IUserModel GetModel() => Model;
}
