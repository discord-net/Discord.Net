using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Reaction :
    IReactionModel,
    IModelSource,
    IModelSourceOf<IEmoteModel>
{
    [JsonPropertyName("count")]
    public int Total { get; set; }

    [JsonPropertyName("me")]
    public bool Me { get; set; }

    [JsonPropertyName("me_burst")]
    public bool IsMeBurst { get; set; }

    [JsonPropertyName("emoji")]
    public required IEmoteModel Emoji { get; set; }

    [JsonPropertyName("count_details")]
    public required ReactionCountDetails CountDetails { get; set; }

    [JsonPropertyName("burst_colors")]
    public required string[] Colors { get; set; }

    ulong? IReactionModel.EmojiId => (Emoji as GuildEmote)?.Id;

    string? IReactionModel.EmojiName => Emoji.Name;

    int IReactionModel.BurstCount => CountDetails.BurstCount;

    int IReactionModel.NormalCount => CountDetails.NormalCount;
    bool IReactionModel.MeBurst => IsMeBurst;
    string[] IReactionModel.BurstColors => Colors;

    public IEnumerable<IEntityModel> GetDefinedModels()
    {
        yield return Emoji;
    }

    IEmoteModel IModelSourceOf<IEmoteModel>.Model => Emoji;
}
