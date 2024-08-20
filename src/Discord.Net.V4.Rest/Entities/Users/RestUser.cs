using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestUserActor :
    RestActor<ulong, RestUser, UserIdentity, IUserModel>,
    IUserActor
{
    internal override UserIdentity Identity { get; }

    [TypeFactory]
    public RestUserActor(
        DiscordRestClient client,
        UserIdentity user
    ) : base(client, user)
    {
        Identity = user | this;
    }

    [SourceOfTruth]
    internal override RestUser CreateEntity(IUserModel model)
        => RestUser.Construct(Client, this, model);

    [SourceOfTruth]
    internal RestDMChannel CreateEntity(IDMChannelModel model)
        => RestDMChannel.Construct(Client, Identity | this, model);
}

[ExtendInterfaceDefaults]
public partial class RestUser :
    RestEntity<ulong>,
    IUser,
    IRestConstructable<RestUser, RestUserActor, IUserModel>
{
    public string? AvatarId => Model.Avatar;

    public ushort Discriminator => ushort.Parse(Model.Discriminator);

    public string Username => Model.Username;

    public string? GlobalName => Model.GlobalName;

    public bool IsBot => Model.IsBot ?? false;

    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;

    [ProxyInterface(
        typeof(IUserActor),
        typeof(IEntityProvider<IUser, IUserModel>)
    )]
    internal virtual RestUserActor Actor { get; }

    internal virtual IUserModel Model => _model;

    private IUserModel _model;

    internal RestUser(
        DiscordRestClient client,
        IUserModel model,
        RestUserActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        _model = model;
    }

    public static RestUser Construct(DiscordRestClient client, RestUserActor actor, IUserModel model)
    {
        return (model, actor) switch
        {
            (ISelfUserModel currentUserModel, RestCurrentUserActor currentUserActor) 
                => RestCurrentUser.Construct(client, currentUserActor, currentUserModel),
            _ => new RestUser(client, model, actor)
        };
    }

    public virtual ValueTask UpdateAsync(IUserModel model, CancellationToken token = default)
    {
        _model = model;

        return ValueTask.CompletedTask;
    }

    public virtual IUserModel GetModel() => Model;
}
