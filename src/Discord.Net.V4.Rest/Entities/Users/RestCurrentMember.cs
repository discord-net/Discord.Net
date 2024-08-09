using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentMemberActor :
    RestMemberActor,
    ICurrentMemberActor,
    IRestActor<ulong, RestCurrentMember, CurrentMemberIdentity>
{
    [SourceOfTruth]
    public override RestCurrentUserVoiceStateActor VoiceState { get; }

    [ProxyInterface, SourceOfTruth]
    private RestCurrentUserActor User { get; }

    [SourceOfTruth]
    internal override CurrentMemberIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(member))]
    public RestCurrentMemberActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CurrentMemberIdentity member,
        CurrentUserVoiceStateIdentity? voiceState = null
    ) : base(client, guild, member, client.CurrentUser.Identity, voiceState)
    {
        Identity = member | this;

        User = client.CurrentUser;

        VoiceState = voiceState?.Actor ?? new(
            client,
            Guild.Identity,
            voiceState ?? CurrentUserVoiceStateIdentity.Of(member.Id),
            Identity
        );
    }

    [SourceOfTruth]
    internal override RestCurrentMember CreateEntity(IMemberModel model)
        => RestCurrentMember.Construct(Client, Guild.Identity, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentMember :
    RestMember,
    ICurrentMember,
    IContextConstructable<RestCurrentMember, IMemberModel, GuildIdentity, DiscordRestClient>
{
    [ProxyInterface]
    internal override RestCurrentMemberActor Actor { get; }

    internal RestCurrentMember(
        DiscordRestClient client,
        GuildIdentity guild,
        IMemberModel model,
        IUserModel userModel,
        RestCurrentMemberActor? actor = null
    ) : base(client, guild, model, userModel, actor)
    {
        Actor = actor ?? new(client, guild, CurrentMemberIdentity.Of(this));
    }

    public new static RestCurrentMember Construct(DiscordRestClient client, GuildIdentity guild, IMemberModel model)
    {
        if (model is not IModelSourceOf<IUserModel?> userModelSource)
            throw new ArgumentException($"Expected {model.GetType()} to be a {typeof(IModelSourceOf<IUserModel?>)}",
                nameof(model));

        if (userModelSource.Model is null)
            throw new ArgumentNullException(nameof(userModelSource), "Expected 'user' to be a non-null model");

        return new RestCurrentMember(client, guild, model, userModelSource.Model);
    }
}
