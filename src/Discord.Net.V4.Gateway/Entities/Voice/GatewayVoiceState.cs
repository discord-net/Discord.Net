using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayVoiceStateActor :
    GatewayCachedActor<ulong, GatewayVoiceState, VoiceStateIdentity, IVoiceStateModel>,
    IVoiceStateActor
{
    [SourceOfTruth, StoreRoot]
    public GatewayGuildActor Guild { get; }

    [SourceOfTruth]
    public virtual GatewayMemberActor Member { get; }

    internal override VoiceStateIdentity Identity { get; }

    public GatewayVoiceStateActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        VoiceStateIdentity voiceState,
        MemberIdentity? member = null
    ) : base(client, voiceState)
    {
        Identity = voiceState | this;

        Guild = client.Guilds >> guild;
        Member = Guild.Members >> (member | voiceState);
    }

    [SourceOfTruth]
    internal virtual GatewayVoiceState CreateEntity(IVoiceStateModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public partial class GatewayVoiceState :
    GatewayCacheableEntity<GatewayVoiceState, ulong, IVoiceStateModel>,
    IVoiceState
{
    [SourceOfTruth]
    public GatewayVoiceChannelActor? Channel { get; private set; }

    public DateTimeOffset? RequestToSpeakTimestamp => Model.RequestToSpeakTimestamp;

    public string SessionId => Model.SessionId;

    public bool IsDeafened => Model.Deaf;

    public bool IsMuted => Model.Mute;

    public bool IsSelfDeafened => Model.SelfDeaf;

    public bool IsSelfMuted => Model.SelfMute;

    public bool IsSuppressed => Model.Suppress;

    public bool IsStreaming => Model.SelfStream ?? false;

    public bool IsVideoing => Model.SelfVideo;

    [ProxyInterface]
    internal virtual GatewayVoiceStateActor Actor { get; }

    internal IVoiceStateModel Model { get; private set; }

    public GatewayVoiceState(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IVoiceStateModel model,
        GatewayVoiceStateActor? actor = null,
        MemberIdentity? member = null
        ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, VoiceStateIdentity.Of(this), member);

        Channel = model.ChannelId.Map(
            (id, guild) => guild.VoiceChannels[id],
            Guild
        );
    }

    public static GatewayVoiceState Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IVoiceStateModel model)
    {
        if(model.UserId == client.CurrentUser.Id)
            return GatewayCurrentUserVoiceState.Construct(client, context, model);

        return new GatewayVoiceState(
            client,
            context.Path.RequireIdentity(Template.T<GuildIdentity>()),
            model,
            context.TryGetActor<GatewayVoiceStateActor>(),
            context.Path.GetIdentity(Template.T<MemberIdentity>())
        );
    }

    public override ValueTask UpdateAsync(
        IVoiceStateModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Channel = Channel.UpdateFrom(
            model.ChannelId,
            (guild, id) => guild.VoiceChannels[id],
            Guild
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IVoiceStateModel GetModel() => Model;
}
