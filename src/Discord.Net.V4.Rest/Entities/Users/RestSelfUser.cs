using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public sealed partial class RestLoadableSelfUserActor(DiscordRestClient client, ulong id) :
    RestSelfUserActor(client, id),
    ILoadableSelfUserActor
{
    [ProxyInterface(typeof(ILoadableEntity<ISelfUser>))]
    internal RestLoadable<ulong, RestSelfUser, ISelfUser, User> Loadable { get; } =
        RestLoadable<ulong, RestSelfUser, ISelfUser, User>.FromConstructable<RestSelfUser>(
            client,
            id,
            Routes.GetCurrentUser
        );
}

[ExtendInterfaceDefaults(
    typeof(ISelfUserActor),
    typeof(IModifiable<ulong, ISelfUserActor, ModifySelfUserProperties, ModifyCurrentUserParams>)
)]
public partial class RestSelfUserActor(DiscordRestClient client, ulong id) :
    RestUserActor(client, id),
    ISelfUserActor;

public partial class RestSelfUser(DiscordRestClient client, ISelfUserModel model, RestSelfUserActor? actor = null) :
    RestUser(client, model),
    ISelfUser,
    IConstructable<RestSelfUser, ISelfUserModel, DiscordRestClient>
{
    [ProxyInterface(typeof(ISelfUserActor))]
    protected override RestSelfUserActor Actor { get; } = actor ?? new(client, model.Id);

    protected override ISelfUserModel Model { get; } = model;

    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.PremiumType ?? PremiumType.None;

    public string Locale => Model.Locale!;

    public static RestSelfUser Construct(DiscordRestClient client, ISelfUserModel model) => new(client, model);
}
