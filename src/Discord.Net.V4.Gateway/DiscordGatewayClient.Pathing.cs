using Discord.Gateway;
using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

public partial class DiscordGatewayClient
{
    [SourceOfTruth]
    public GatewayCurrentUserActor CurrentUser => throw new NotImplementedException();

    [SourceOfTruth]
    public GuildsPager Guilds { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayChannelActor, ulong, GatewayChannel> Channels { get; }

    [SourceOfTruth]
    public GatewayIndexableActor<GatewayUserActor, ulong, GatewayUser> Users { get; }

    public IIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

    public IEnumerableIndexableActor<IStickerPackActor, ulong, IStickerPack> StickerPacks
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IIndexableActor<IStickerActor, ulong, ISticker> Stickers
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
}
