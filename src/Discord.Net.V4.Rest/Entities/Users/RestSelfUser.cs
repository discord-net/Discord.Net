using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[method: TypeFactory]
[ExtendInterfaceDefaults(
    typeof(ICurrentUserActor)
)]
public partial class RestCurrentUserActor(
    DiscordRestClient client,
    SelfUserIdentity user
) :
    RestUserActor(client, user),
    ICurrentUserActor
{
    [CovariantOverride]
    [SourceOfTruth]
    internal RestCurrentUser CreateEntity(ISelfUserModel model)
        => RestCurrentUser.Construct(Client, model);

    [SourceOfTruth]
    internal RestPartialGuild CreateEntity(IPartialGuildModel model)
        => RestPartialGuild.Construct(Client, model);

    [SourceOfTruth]
    internal RestMember CreateEntity(IMemberModel model, ulong guildId)
        => RestMember.Construct(Client, GuildIdentity.Of(guildId), model);
}

public partial class RestCurrentUser :
    RestUser,
    ICurrentUser,
    IConstructable<RestCurrentUser, ISelfUserModel, DiscordRestClient>
{
    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.PremiumType ?? PremiumType.None;

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
        RestCurrentUserActor? actor = null
    ) : base(client, model)
    {
        Actor = actor ?? new(client, SelfUserIdentity.Of(this));
        _model = model;
    }

    public static RestCurrentUser Construct(DiscordRestClient client, ISelfUserModel model) => new(client, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(ISelfUserModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override ISelfUserModel GetModel() => Model;
}
