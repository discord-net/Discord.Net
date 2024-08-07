namespace Discord.Models;

public interface IGuildStickersUpdatedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IEnumerable<IGuildStickerModel> Stickers { get; }
}
