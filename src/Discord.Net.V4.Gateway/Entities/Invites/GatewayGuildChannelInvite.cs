using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayGuildChannelInviteActor :
    GatewayGuildInviteActor,
    IGuildChannelInviteActor,
    IGatewayCachedActor<string, GatewayGuildChannelInvite, GuildChannelInviteIdentity, IInviteModel>
{
    [SourceOfTruth] public GatewayGuildChannelActor Channel { get; }

    [SourceOfTruth] internal override GuildChannelInviteIdentity Identity { get; }

    [TypeFactory]
    public GatewayGuildChannelInviteActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        GuildChannelInviteIdentity invite
    ) : base(client, guild, invite)
    {
        Identity = invite | this;

        Channel = Guild.Channels[channel];
    }
}

public sealed partial class GatewayGuildChannelInvite :
    GatewayGuildInvite,
    IGuildChannelInvite,
    ICacheableEntity<GatewayGuildChannelInvite, string, IInviteModel>
{
    [ProxyInterface] internal override GatewayGuildChannelInviteActor Actor { get; }

    internal GatewayGuildChannelInvite(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        IInviteModel model,
        GatewayGuildChannelInviteActor? actor = null
    ) : base(client, guild, model, actor)
    {
        Actor = actor ?? Guild.Channels[channel].Invites[Id];
    }

    public new static GatewayGuildChannelInvite Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IInviteModel model
    ) => new(
        client,
        context.Path.RequireIdentity(Template.T<GuildIdentity>()),
        context.Path.RequireIdentity(Template.T<GuildChannelIdentity>()),
        model,
        context.TryGetActor<GatewayGuildChannelInviteActor>()
    );
}
