using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayStickerItem(
    DiscordGatewayClient client,
    IStickerItemModel model
) :
    GatewayEntity<ulong>(client, model.Id),
    IStickerItem
{
    public string Name => model.Name;
    public StickerFormatType Format => (StickerFormatType)model.FormatType;

    public IStickerItemModel GetModel() => model;

    // ReSharper disable once ParameterHidesPrimaryConstructorParameter
    [SourceOfTruth]
    internal GatewaySticker CreateEntity(IStickerModel model)
    {
        var path = CachePathable.Empty;

        if (model is IGuildStickerModel guildStickerModel)
            path = new CachePathable {Client.Guilds[guildStickerModel.GuildId]};

        return GatewaySticker.Construct(Client, new GatewayConstructionContext(path), model);
    }
}
