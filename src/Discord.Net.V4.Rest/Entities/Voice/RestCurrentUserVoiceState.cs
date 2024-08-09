using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentUserVoiceStateActor :
    RestVoiceStateActor,
    ICurrentUserVoiceStateActor,
    IRestActor<ulong, RestCurrentUserVoiceState, CurrentUserVoiceStateIdentity>
{
    [SourceOfTruth]
    internal override CurrentUserVoiceStateIdentity Identity { get; }

    public RestCurrentUserVoiceStateActor(
        DiscordRestClient client,
        GuildIdentity guild,
        CurrentUserVoiceStateIdentity voiceState,
        MemberIdentity? member = null
    ) : base(client, guild, voiceState, member)
    {
        Identity = voiceState | this;
    }

    [SourceOfTruth]
    internal override RestCurrentUserVoiceState CreateEntity(IVoiceStateModel model)
        => RestCurrentUserVoiceState.Construct(Client, new(Guild.Identity, Member.Identity), model);
}

[ExtendInterfaceDefaults]
public sealed partial class RestCurrentUserVoiceState :
    RestVoiceState,
    ICurrentUserVoiceState,
    IContextConstructable<RestCurrentUserVoiceState, IVoiceStateModel, RestVoiceState.Context, DiscordRestClient>
{
    [ProxyInterface]
    internal override RestCurrentUserVoiceStateActor Actor { get; }

    public RestCurrentUserVoiceState(
        DiscordRestClient client,
        GuildIdentity guild,
        IVoiceStateModel model,
        RestCurrentUserVoiceStateActor? actor = null,
        MemberIdentity? member = null
    ) : base(client, guild, model, actor, member)
    {
        Actor = actor ?? new(client, guild, CurrentUserVoiceStateIdentity.Of(this), member);
    }

    public new static RestCurrentUserVoiceState Construct(DiscordRestClient client, Context context,
        IVoiceStateModel model)
        => new(client, context.Guild, model, member: context.Member);
}
