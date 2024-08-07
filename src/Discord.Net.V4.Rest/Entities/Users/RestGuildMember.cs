using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

[method: TypeFactory(LastParameter = nameof(member))]
[ExtendInterfaceDefaults]
public partial class RestMemberActor(
    DiscordRestClient client,
    GuildIdentity guild,
    MemberIdentity member,
    UserIdentity? user = null
):
    RestUserActor(client, user ?? member.Cast<RestUser, RestUserActor, IUserModel>()),
    IMemberActor,
    IRestActor<ulong, RestMember, MemberIdentity>
{
    public new MemberIdentity Identity { get; } = member;

    [SourceOfTruth]
    public RestGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    [SourceOfTruth]
    internal RestMember CreateEntity(IMemberModel model)
        => RestMember.Construct(Client, Guild.Identity, model);

    IUserActor IUserRelationship.User => this;
}

public sealed partial class RestMember :
    RestUser,
    IMember,
    IContextConstructable<RestMember, IMemberModel, GuildIdentity, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IRole> Roles => throw new NotImplementedException();

    public DateTimeOffset? JoinedAt => Model.JoinedAt;

    public string? Nickname => Model.Nickname;

    public string? GuildAvatarId => Model.Avatar;

    public DateTimeOffset? PremiumSince => Model.PremiumSince;

    public bool? IsPending => Model.IsPending;

    public DateTimeOffset? TimedOutUntil => Model.CommunicationsDisabledUntil;

    public GuildMemberFlags Flags => (GuildMemberFlags)Model.Flags;

    [ProxyInterface(typeof(IMemberActor))]
    internal override RestMemberActor Actor { get; }

    internal new IMemberModel Model { get; private set; }

    internal RestMember(
        DiscordRestClient client,
        GuildIdentity guild,
        IMemberModel model,
        IUserModel userModel,
        RestMemberActor? actor = null
    ) : base(client, userModel)
    {
        Actor = actor ?? new(
            client,
            guild,
            MemberIdentity.Of(this),
            UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, model.Id, client)
        );
        Model = model;
    }

    public static RestMember Construct(DiscordRestClient client, GuildIdentity guild, IMemberModel model)
    {
        if(model is not IModelSourceOf<IUserModel?> userModelSource)
            throw new ArgumentException($"Expected {model.GetType()} to be a {typeof(IModelSourceOf<IUserModel?>)}", nameof(model));

        if (userModelSource.Model is null)
            throw new ArgumentNullException(nameof(userModelSource), "Expected 'user' to be a non-null model");

        return new RestMember(client, guild, model, userModelSource.Model);
    }

    public ValueTask UpdateAsync(IMemberModel model, CancellationToken token = default)
    {
        Model = model;

        if (model is IModelSourceOf<IUserModel?> {Model: { } userModel})
            return base.UpdateAsync(userModel, token);

        return ValueTask.CompletedTask;
    }

    public override ValueTask UpdateAsync(IUserModel model, CancellationToken token = default)
        => throw new InvalidOperationException(
            $"Cannot update a {nameof(RestMember)} with a plain {nameof(IUserModel)}");

    public new IMemberModel GetModel() => Model;
}
