using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ForumThreadMessage
{
    [JsonProperty("content")]
    public Optional<string> Content { get; set; }

    [JsonProperty("nonce")]
    public Optional<string> Nonce { get; set; }

    [JsonProperty("embeds")]
    public Optional<Embed[]> Embeds { get; set; }

    [JsonProperty("allowed_mentions")]
    public Optional<AllowedMentions> AllowedMentions { get; set; }

    [JsonProperty("components")]
    public Optional<API.ActionRowComponent[]> Components { get; set; }

    [JsonProperty("sticker_ids")]
    public Optional<ulong[]> Stickers { get; set; }

    [JsonProperty("flags")]
    public Optional<MessageFlags> Flags { get; set; }

    [JsonProperty("poll")]
    public Optional<CreatePollParams> Poll { get; set; }
}
