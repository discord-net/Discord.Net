using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestMemberActor :
    RestUserActor,
    IMemberActor,
    IRestActor<RestMemberActor, ulong, RestMember, IMemberModel>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }
    
    [SourceOfTruth]
    public RestRoleActor.Indexable.BackLink<RestMemberActor> Roles { get; }

    [SourceOfTruth] public virtual RestVoiceStateActor VoiceState { get; }

    [SourceOfTruth] internal new virtual MemberIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(member))]
    public RestMemberActor(
        DiscordRestClient client,
        GuildIdentity guild,
        MemberIdentity member,
        UserIdentity? user = null,
        VoiceStateIdentity? voiceState = null
    ) : base(client, user ?? member.Cast<RestUser, RestUserActor, IUserModel>())
    {
        member = Identity = member | this;

        Guild = guild.Actor ?? new(client, guild);

        VoiceState = voiceState?.Actor ?? new(
            client,
            Guild.Identity,
            voiceState ?? VoiceStateIdentity.Of(member.Id),
            member
        );

        Roles = new(this, client, Guild.Roles);
    }

    [SourceOfTruth]
    internal virtual RestMember CreateEntity(IMemberModel model)
        => RestMember.Construct(Client, this, model);

    IUserActor IUserRelationship.User => this;
}

[ExtendInterfaceDefaults]
public partial class RestMember :
    RestUser,
    IMember,
    IRestConstructable<RestMember, RestMemberActor, IMemberModel>
{
    [SourceOfTruth]
    public RestRoleActor.Defined.Indexable.BackLink<RestMember> Roles { get; }

    public DateTimeOffset? JoinedAt => Model.JoinedAt;

    public string? Nickname => Model.Nickname;

    public string? GuildAvatarId => Model.Avatar;

    public DateTimeOffset? PremiumSince => Model.PremiumSince;

    public bool? IsPending => Model.IsPending;

    public DateTimeOffset? TimedOutUntil => Model.CommunicationsDisabledUntil;

    public GuildMemberFlags Flags => (GuildMemberFlags)Model.Flags;

    [ProxyInterface(typeof(IMemberActor))] internal override RestMemberActor Actor { get; }

    internal new IMemberModel Model { get; private set; }

    internal RestMember(
        DiscordRestClient client,
        IMemberModel model,
        IUserModel userModel,
        RestMemberActor actor
    ) : base(client, userModel, actor)
    {
        Actor = actor;
        Model = model;

        Roles = new(
            this,
            client,
            actor.Roles,
            model.RoleIds
        );
    }

    public static RestMember Construct(DiscordRestClient client, RestMemberActor actor, IMemberModel model)
    {
        if (actor is RestCurrentMemberActor currentMemberActor)
            return RestCurrentMember.Construct(client, currentMemberActor, model);
        
        if(model.Id == client.Users.Current.Id)
            return RestCurrentMember.Construct(client, actor.Guild.Members.Current, model);
        
        if (model is not IModelSourceOf<IUserModel?> userModelSource)
            throw new ArgumentException($"Expected {model.GetType()} to be a {typeof(IModelSourceOf<IUserModel?>)}",
                nameof(model));

        if (userModelSource.Model is null)
            throw new ArgumentNullException(nameof(userModelSource), "Expected 'user' to be a non-null model");

        return new RestMember(client, model, userModelSource.Model, actor);
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
