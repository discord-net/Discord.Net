using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Sticker : IStickerModel, IGuildStickerModel, IEntityModelSource
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("pack_id")]
    public Optional<ulong> PackId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("tags")]
    public required string Tags { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("format_type")]
    public int FormatType { get; set; }

    [JsonPropertyName("available")]
    public Optional<bool> IsAvailable { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    [JsonPropertyName("sort_value")]
    public int? SortValue { get; set; }

    ulong? IStickerModel.PackId => PackId;

    bool? IGuildStickerModel.Available => IsAvailable;

    ulong IGuildStickerModel.GuildId => GuildId;

    ulong? IGuildStickerModel.AuthorId => User.Map(v => v.Id);

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (User) yield return User.Value;
    }
}
