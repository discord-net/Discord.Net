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
    public GatewayIndexableActor<GatewayChannelActor, ulong, GatewayChannel> Channels { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayUserActor, ulong, GatewayUser> Users { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayWebhookActor, ulong, GatewayWebhook> Webhooks { get; }

    [SourceOfTruth]
    public StickerPacks StickerPacks { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayStickerActor, ulong, GatewaySticker> Stickers { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayInviteActor, string, GatewayInvite> Invites { get; }
}
