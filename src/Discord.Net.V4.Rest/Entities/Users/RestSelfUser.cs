using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[method: TypeFactory]
[ExtendInterfaceDefaults(
    typeof(ISelfUserActor)
)]
public partial class RestSelfUserActor(
    DiscordRestClient client,
    SelfUserIdentity user
) :
    RestUserActor(client, user),
    ISelfUserActor
{
    [CovariantOverride]
    [SourceOfTruth]
    internal RestSelfUser CreateEntity(ISelfUserModel model)
        => RestSelfUser.Construct(Client, model);

    [SourceOfTruth]
    internal RestPartialGuild CreateEntity(IPartialGuildModel model)
        => RestPartialGuild.Construct(Client, model);

    [SourceOfTruth]
    internal RestGuildMember CreateEntity(IMemberModel model, ulong guildId)
        => RestGuildMember.Construct(Client, GuildIdentity.Of(guildId), model);
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

    [CovariantOverride]
    public ValueTask UpdateAsync(ISelfUserModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override ISelfUserModel GetModel() => Model;
}
