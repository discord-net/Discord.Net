using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestVoiceStateActor :
    RestActor<RestVoiceStateActor, ulong, RestVoiceState, IVoiceStateModel>,
    IVoiceStateActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public virtual RestMemberActor Member { get; }

    internal override VoiceStateIdentity Identity { get; }

    public RestVoiceStateActor(
        DiscordRestClient client,
        GuildIdentity guild,
        VoiceStateIdentity voiceState,
        MemberIdentity? member = null
    ) : base(client, voiceState)
    {
        Identity = voiceState | this;

        Guild = client.Guilds[guild];
        Member = Guild.Members[member | voiceState];
    }

    [SourceOfTruth]
    internal override RestVoiceState CreateEntity(IVoiceStateModel model)
        => RestVoiceState.Construct(Client, this, model);
}

[ExtendInterfaceDefaults]
public partial class RestVoiceState :
    RestEntity<ulong>,
    IVoiceState,
    IRestConstructable<RestVoiceState, RestVoiceStateActor, IVoiceStateModel>
{
    public readonly record struct Context(GuildIdentity Guild, MemberIdentity? Member = null);

    [SourceOfTruth] public RestVoiceChannelActor? Channel { get; private set; }

    public DateTimeOffset? RequestToSpeakTimestamp => Model.RequestToSpeakTimestamp;

    public string SessionId => Model.SessionId;

    public bool IsDeafened => Model.Deaf;

    public bool IsMuted => Model.Mute;

    public bool IsSelfDeafened => Model.SelfDeaf;

    public bool IsSelfMuted => Model.SelfMute;

    public bool IsSuppressed => Model.Suppress;

    public bool IsStreaming => Model.SelfStream ?? false;

    public bool IsVideoing => Model.SelfVideo;

    [ProxyInterface] internal virtual RestVoiceStateActor Actor { get; }

    internal IVoiceStateModel Model { get; }

    internal RestVoiceState(
        DiscordRestClient client,
        IVoiceStateModel model,
        RestVoiceStateActor actor
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor;

        Channel = model.ChannelId.Map(
            static (id, actor) => actor.Guild.Channels.Voice[id],
            actor
        );
    }

    public static RestVoiceState Construct(
        DiscordRestClient client,
        RestVoiceStateActor actor,
        IVoiceStateModel model)
    {
        return model.UserId == client.Users.Current.Id
            ? RestCurrentUserVoiceState
                .Construct(
                    client,
                    actor as RestCurrentUserVoiceStateActor ?? actor.Guild.Members.Current.VoiceState,
                    model
                )
            : new RestVoiceState(client, model, actor);
    }

    public ValueTask UpdateAsync(IVoiceStateModel model, CancellationToken token = default)
    {
        Channel = Channel.UpdateFrom(
            model.ChannelId,
            RestVoiceChannelActor.Factory,
            Client,
            Guild.Identity
        );

        return ValueTask.CompletedTask;
    }

    public IVoiceStateModel GetModel() => Model;
}