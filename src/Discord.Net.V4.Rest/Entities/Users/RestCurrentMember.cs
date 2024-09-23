using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
[BackLinkable]
public sealed partial class RestCurrentMemberActor :
    RestMemberActor,
    ICurrentMemberActor,
    IRestActor<RestCurrentMemberActor, ulong, RestCurrentMember, IMemberModel>
{
    [SourceOfTruth] public override RestCurrentUserVoiceStateActor VoiceState { get; }

    [ProxyInterface, SourceOfTruth] private RestCurrentUserActor User { get; }

    [SourceOfTruth] internal override CurrentMemberIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(member))]
    public RestCurrentMemberActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CurrentMemberIdentity member,
        CurrentUserVoiceStateIdentity? voiceState = null
    ) : base(client, guild, member, client.Users.Current.Identity, voiceState)
    {
        Identity = member | this;

        User = client.Users.Current;

        VoiceState = voiceState?.Actor ?? new(
            client,
            Guild.Identity,
            voiceState ?? CurrentUserVoiceStateIdentity.Of(member.Id)
        );
    }

    [SourceOfTruth]
    internal override RestCurrentMember CreateEntity(IMemberModel model)
        => RestCurrentMember.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentMember :
    RestMember,
    ICurrentMember,
    IRestConstructable<RestCurrentMember, RestCurrentMemberActor, IMemberModel>
{
    [ProxyInterface] internal override RestCurrentMemberActor Actor { get; }

    internal RestCurrentMember(
        DiscordRestClient client,
        IMemberModel model,
        IUserModel userModel,
        RestCurrentMemberActor actor
    ) : base(client, model, userModel, actor)
    {
        Actor = actor;
    }

    public static RestCurrentMember Construct(
        DiscordRestClient client,
        RestCurrentMemberActor actor,
        IMemberModel model)
    {
        if (model is not IModelSourceOf<IUserModel?> userModelSource)
            throw new ArgumentException($"Expected {model.GetType()} to be a {typeof(IModelSourceOf<IUserModel?>)}",
                nameof(model));

        if (userModelSource.Model is null)
            throw new ArgumentNullException(nameof(userModelSource), "Expected 'user' to be a non-null model");

        return new RestCurrentMember(client, model, userModelSource.Model, actor);
    }
}