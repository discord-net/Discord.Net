using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestVoiceStateActor :
    RestActor<ulong, RestVoiceState, VoiceStateIdentity>,
    IVoiceStateActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public virtual RestMemberActor Member { get; }

    internal override VoiceStateIdentity Identity { get; }

    public RestVoiceStateActor(DiscordRestClient client,
        GuildIdentity guild,
        VoiceStateIdentity voiceState,
        MemberIdentity? member = null) : base(client, voiceState)
    {
        Identity = voiceState | this;

        Guild = guild.Actor ?? new(client, guild);
        Member = member?.Actor ?? new(client, guild, MemberIdentity.Of(voiceState.Id));
    }

    [SourceOfTruth]
    internal virtual RestVoiceState CreateEntity(IVoiceStateModel model)
        => RestVoiceState.Construct(Client, new(Guild.Identity, Member.Identity), model);
}

[ExtendInterfaceDefaults]
public partial class RestVoiceState :
    RestEntity<ulong>,
    IVoiceState,
    IContextConstructable<RestVoiceState, IVoiceStateModel, RestVoiceState.Context, DiscordRestClient>
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

    public RestVoiceState(
        DiscordRestClient client,
        GuildIdentity guild,
        IVoiceStateModel model,
        RestVoiceStateActor? actor = null,
        MemberIdentity? member = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, VoiceStateIdentity.Of(this), member);

        Channel = model.ChannelId.Map(
            static (id, client, guild) => new RestVoiceChannelActor(client, guild, VoiceChannelIdentity.Of(id)),
            client,
            guild
        );
    }

    public static RestVoiceState Construct(DiscordRestClient client, Context context, IVoiceStateModel model)
    {
        return model.UserId == client.CurrentUser.Id
            ? RestCurrentUserVoiceState.Construct(client, context, model)
            : new RestVoiceState(client, context.Guild, model, member: context.Member);
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
