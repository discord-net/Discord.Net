using Discord.Models;
using Discord.Rest.Extensions;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayGuildInviteActor :
    GatewayInviteActor,
    IGuildInviteActor,
    IGatewayCachedActor<string, GatewayGuildInvite, GuildInviteIdentity, IInviteModel>
{
    [SourceOfTruth] public GatewayGuildActor Guild { get; }

    [SourceOfTruth] internal override GuildInviteIdentity Identity { get; }

    public GatewayGuildInviteActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildInviteIdentity invite
    ) : base(client, invite)
    {
        Identity = invite | this;

        Guild = client.Guilds >> guild;
    }
}

[ExtendInterfaceDefaults]
public partial class GatewayGuildInvite :
    GatewayInvite,
    IGuildInvite,
    ICacheableEntity<GatewayGuildInvite, string, IInviteModel>
{
    [SourceOfTruth] public GatewayGuildScheduledEventActor? GuildScheduledEvent { get; private set; }

    [ProxyInterface] internal override GatewayGuildInviteActor Actor { get; }

    internal GatewayGuildInvite(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IInviteModel model,
        GatewayGuildInviteActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? (client.Guilds >> guild).Invites[model.Id];

        UpdateLinkedActors(model);
    }

    public new static GatewayGuildInvite Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IInviteModel model)
    {
        if (model.ChannelId.HasValue)
            return GatewayGuildChannelInvite.Construct(client, context, model);

        return new GatewayGuildInvite(
            client,
            context.Path.GetIdentity(Template.Of<GuildIdentity>()) ??
            (
                model.GuildId.HasValue
                    ? GuildIdentity.Of(model.GuildId!.Value)
                    : throw new ArgumentException("Expected a guild either on the model or in the context")
            ),
            model,
            context.TryGetActor<GatewayGuildInviteActor>()
        );
    }

    private void UpdateLinkedActors(IInviteModel model)
    {
        GuildScheduledEvent = GuildScheduledEvent.UpdateFrom(
            model.ScheduledEventId,
            (guild, scheduledEvent) => guild.ScheduledEvents[scheduledEvent],
            Actor.Guild
        );
    }

    public override ValueTask UpdateAsync(
        IInviteModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        UpdateLinkedActors(model);

        return base.UpdateAsync(model, false, token);
    }

    ValueTask IUpdatable<IInviteModel>.UpdateAsync(IInviteModel model, CancellationToken token)
        => UpdateAsync(model, true, token);
}
