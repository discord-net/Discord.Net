using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(ICustomEmoteModel.Id))]
[HasPartialVariant]
public abstract class Emote : IEmoteModel
{
    [JsonPropertyName("name"), NullableInPartial]
    public required string Name { get; set; }

    protected abstract DiscordEmojiId DiscordEmojiId { get; }

    DiscordEmojiId IEntityModel<DiscordEmojiId>.Id => DiscordEmojiId;
}