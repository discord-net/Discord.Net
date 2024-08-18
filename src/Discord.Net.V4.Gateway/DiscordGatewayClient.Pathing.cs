using Discord.Gateway;
using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

public partial class DiscordGatewayClient
{
    [SourceOfTruth]
    public GatewayCurrentUserActor CurrentUser { get; }

    [SourceOfTruth]
    public GuildsPager Guilds { get; }

    [SourceOfTruth]
    public GatewayIndexableLink<GatewayChannelActor, ulong, GatewayChannel> Channels { get; }

    [SourceOfTruth]
    public GatewayIndexableLink<GatewayUserActor, ulong, GatewayUser> Users { get; }

    [SourceOfTruth]
    public GatewayIndexableLink<GatewayWebhookActor, ulong, GatewayWebhook> Webhooks { get; }

    [SourceOfTruth]
    public StickerPacks StickerPacks { get; }

    [SourceOfTruth]
    public GatewayIndexableLink<GatewayStickerActor, ulong, GatewaySticker> Stickers { get; }

    [SourceOfTruth]
    public GatewayIndexableLink<GatewayInviteActor, string, GatewayInvite> Invites { get; }
}
