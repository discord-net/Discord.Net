using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class RestLoadableUserActor(DiscordRestClient client, ulong id, IUserModel? value = null) :
    RestUserActor(client, id),
    ILoadableUserActor
{
    [ProxyInterface(typeof(ILoadableEntity<IUser>))]
    internal RestLoadable<ulong, RestUser, IUser, IUserModel> Loadable { get; } =
        RestLoadable<ulong, RestUser, IUser, IUserModel>.FromConstructable<RestUser>(
            client,
            id,
            Routes.GetUser,
            value
        );
}

[ExtendInterfaceDefaults(typeof(IUserActor))]
public partial class RestUserActor(DiscordRestClient client, ulong id) :
    RestActor<ulong, RestSelfUser>(client, id),
    IUserActor;

public partial class RestUser(DiscordRestClient client, IUserModel model, RestUserActor? actor = null) :
    RestEntity<ulong>(client, model.Id),
    IUser,
    IConstructable<RestUser, IUserModel, DiscordRestClient>
{
    [ProxyInterface(typeof(IUserActor))]
    protected virtual RestUserActor Actor { get; } = actor ?? new(client, model.Id);

    protected virtual IUserModel Model { get; } = model;

    public static RestUser Construct(DiscordRestClient client, IUserModel model)
    {
        return model switch
        {
            ISelfUserModel selfUserModel => RestSelfUser.Construct(client, selfUserModel),
            _ => new RestUser(client, model)
        };
    }

    public string? AvatarId => Model.Avatar;

    public ushort Discriminator => Model.Discriminator;

    public string Username => Model.Username;

    public string? GlobalName => Model.GlobalName;

    public bool IsBot => Model.IsBot ?? false;

    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;
}
