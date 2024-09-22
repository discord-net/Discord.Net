using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestCurrentUserActor :
    RestUserActor,
    ICurrentUserActor,
    IRestActor<RestCurrentUserActor, ulong, RestCurrentUser, ISelfUserModel>
{
    [SourceOfTruth] internal override SelfUserIdentity Identity { get; }

    [TypeFactory]
    public RestCurrentUserActor(
        DiscordRestClient client,
        SelfUserIdentity user
    ) : base(client, user)
    {
        Identity = user | this;
    }

    [CovariantOverride]
    [SourceOfTruth]
    internal RestCurrentUser CreateEntity(ISelfUserModel model)
        => RestCurrentUser.Construct(Client, this, model);

    [SourceOfTruth]
    internal RestPartialGuild CreateEntity(IPartialGuildModel model)
        => RestPartialGuild.Construct(Client, model);

    [SourceOfTruth]
    internal RestMember CreateEntity(IMemberModel model, ulong guildId)
        => RestMember.Construct(Client, Client.Guilds[guildId].Members[model.Id], model);
}

[ExtendInterfaceDefaults]
public partial class RestCurrentUser :
    RestUser,
    ICurrentUser,
    IRestConstructable<RestCurrentUser, RestCurrentUserActor, ISelfUserModel>
{
    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?) Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?) Model.PremiumType ?? PremiumType.None;

    public string Locale => Model.Locale!;

    [ProxyInterface(
        typeof(ICurrentUserActor),
        typeof(IEntityProvider<ICurrentUser, ISelfUserModel>)
    )]
    internal override RestCurrentUserActor Actor { get; }

    internal override ISelfUserModel Model => _model;

    private ISelfUserModel _model;

    internal RestCurrentUser(
        DiscordRestClient client,
        ISelfUserModel model,
        RestCurrentUserActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
        _model = model;
    }

    public static RestCurrentUser Construct(
        DiscordRestClient client,
        RestCurrentUserActor actor,
        ISelfUserModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(ISelfUserModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override ISelfUserModel GetModel() => Model;
}