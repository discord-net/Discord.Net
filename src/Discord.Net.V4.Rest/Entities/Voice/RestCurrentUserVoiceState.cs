using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentUserVoiceStateActor :
    RestVoiceStateActor,
    ICurrentUserVoiceStateActor,
    IRestActor<RestCurrentUserVoiceStateActor, ulong, RestCurrentUserVoiceState, IVoiceStateModel>
{
    [SourceOfTruth] public override RestCurrentMemberActor Member => Guild.Members.Current;

    [SourceOfTruth] internal override CurrentUserVoiceStateIdentity Identity { get; }

    public RestCurrentUserVoiceStateActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CurrentUserVoiceStateIdentity voiceState
    ) : base(client, guild, voiceState, client.Guilds[guild].Members.Current.Identity)
    {
        Identity = voiceState | this;
    }

    [SourceOfTruth]
    internal override RestCurrentUserVoiceState CreateEntity(IVoiceStateModel model)
        => RestCurrentUserVoiceState.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentUserVoiceState :
    RestVoiceState,
    ICurrentUserVoiceState,
    IRestConstructable<RestCurrentUserVoiceState, RestCurrentUserVoiceStateActor, IVoiceStateModel>
{
    [ProxyInterface] internal override RestCurrentUserVoiceStateActor Actor { get; }

    internal RestCurrentUserVoiceState(
        DiscordRestClient client,
        IVoiceStateModel model,
        RestCurrentUserVoiceStateActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
    }

    public static RestCurrentUserVoiceState Construct(
        DiscordRestClient client,
        RestCurrentUserVoiceStateActor actor,
        IVoiceStateModel model
    ) => new(client, model, actor);
}