namespace Discord.Models;

public interface IStickerPackModel : IEntityModel<ulong>
{
    IEnumerable<IStickerModel> Stickers { get; }
    string Name { get; }
    ulong SkuId { get; }
    ulong? CoverStickerId { get; }
    string Description { get; }
    ulong? BannerAssetId { get; }
}
