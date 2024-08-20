using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Reaction :
    IReactionModel
{
    [JsonPropertyName("count")]
    public int Total { get; set; }

    [JsonPropertyName("me")]
    public bool Me { get; set; }

    [JsonPropertyName("me_burst")]
    public bool IsMeBurst { get; set; }

    [JsonPropertyName("emoji")]
    public required DiscordEmojiId Emoji { get; set; }

    [JsonPropertyName("count_details")]
    public required ReactionCountDetails CountDetails { get; set; }

    [JsonPropertyName("burst_colors")]
    public required string[] Colors { get; set; }

    int IReactionModel.BurstCount => CountDetails.BurstCount;

    int IReactionModel.NormalCount => CountDetails.NormalCount;
    bool IReactionModel.MeBurst => IsMeBurst;
    string[] IReactionModel.BurstColors => Colors;

    DiscordEmojiId IEntityModel<DiscordEmojiId>.Id => Emoji;
}
