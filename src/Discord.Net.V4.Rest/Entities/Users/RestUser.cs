using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestUserActor :
    RestActor<ulong, RestCurrentUser, UserIdentity>,
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
    internal virtual RestUser CreateEntity(IUserModel model)
        => RestUser.Construct(Client, model);

    [SourceOfTruth]
    internal RestDMChannel CreateEntity(IDMChannelModel model)
        => RestDMChannel.Construct(Client, Identity | this, model);
}

[ExtendInterfaceDefaults]
public partial class RestUser :
    RestEntity<ulong>,
    IUser,
    IConstructable<RestUser, IUserModel, DiscordRestClient>
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
        RestUserActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, UserIdentity.Of(this));
        _model = model;
    }

    public static RestUser Construct(DiscordRestClient client, IUserModel model)
    {
        return model switch
        {
            ISelfUserModel selfUserModel => RestCurrentUser.Construct(client, selfUserModel),
            _ => new RestUser(client, model)
        };
    }

    public virtual ValueTask UpdateAsync(IUserModel model, CancellationToken token = default)
    {
        _model = model;

        return ValueTask.CompletedTask;
    }

    public virtual IUserModel GetModel() => Model;
}
