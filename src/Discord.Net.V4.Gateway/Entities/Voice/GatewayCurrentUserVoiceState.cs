using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayCurrentUserVoiceStateActor :
    GatewayVoiceStateActor,
    ICurrentUserVoiceStateActor,
    IGatewayCachedActor<ulong, GatewayCurrentUserVoiceState, CurrentUserVoiceStateIdentity, IVoiceStateModel>
{
    [SourceOfTruth] public override GatewayCurrentMemberActor Member { get; }

    [SourceOfTruth] internal override CurrentUserVoiceStateIdentity Identity { get; }

    public GatewayCurrentUserVoiceStateActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        CurrentUserVoiceStateIdentity voiceState,
        CurrentMemberIdentity? member = null
    ) : base(client, guild, voiceState, member | voiceState)
    {
        Identity = voiceState | this;

        Member = Guild.CurrentMember;
    }

    [SourceOfTruth]
    internal override GatewayCurrentUserVoiceState CreateEntity(IVoiceStateModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayCurrentUserVoiceState :
    GatewayVoiceState,
    ICurrentUserVoiceState,
    ICacheableEntity<GatewayCurrentUserVoiceState, ulong, IVoiceStateModel>
{
    [ProxyInterface] internal override GatewayCurrentUserVoiceStateActor Actor { get; }

    public GatewayCurrentUserVoiceState(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IVoiceStateModel model,
        GatewayCurrentUserVoiceStateActor? actor = null,
        CurrentMemberIdentity? member = null
    ) : base(client, guild, model, actor, member)
    {
        Actor = actor ?? new(client, guild, CurrentUserVoiceStateIdentity.Of(this), member);
    }

    public new static GatewayCurrentUserVoiceState Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IVoiceStateModel model
    ) => new(
        client,
        context.Path.RequireIdentity(Template.T<GuildIdentity>()),
        model,
        context.TryGetActor<GatewayCurrentUserVoiceStateActor>(),
        context.Path.GetIdentity(Template.T<CurrentMemberIdentity>())
    );

    public override ValueTask UpdateAsync(
        IVoiceStateModel model,
        bool updateCache = true,
        CancellationToken token = default)
        => updateCache
            ? UpdateCacheAsync(this, model, token)
            : base.UpdateAsync(model, false, token);

    // required for most-specific
    ValueTask IUpdatable<IVoiceStateModel>.UpdateAsync(IVoiceStateModel model, CancellationToken token)
        => UpdateAsync(model, true, token);
}
