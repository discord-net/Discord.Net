using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildEmotesUpdated : IGuildEmotesUpdatedPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("emojis")]
    public required CustomEmote[] Emotes { get; set; }

    IEnumerable<ICustomEmoteModel> IGuildEmotesUpdatedPayloadData.Emotes => Emotes;
}
