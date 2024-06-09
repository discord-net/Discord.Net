using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class StickerPack : IEntityModelSource
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("stickers")]
    public required Sticker[] Stickers { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("cover_sticker_id")]
    public Optional<ulong> CoverStickerId { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("banner_asset_id")]
    public Optional<ulong> BannerAssetId { get; set; }

    public IEnumerable<IEntityModel> GetEntities() => Stickers;
}
