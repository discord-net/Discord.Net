using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class ReactionIdConverter : JsonConverter<DiscordEmojiId>
{
    public override DiscordEmojiId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String when reader.GetString() is { } rawValue:
                if (!DiscordEmojiId.TryParse(rawValue, out var result))
                    throw new JsonException("Invalid emoji identifier");
                return result;
            case JsonTokenType.StartObject when JsonNode.Parse(ref reader) is JsonObject obj:
                ulong? id = null;
                string? name = null;
                bool isAnimated = false;

                if (obj.TryGetPropertyValue("id", out var idNode) && idNode is not null)
                {
                    id = idNode.GetValueKind() switch
                    {
                        JsonValueKind.Number => idNode.Deserialize<ulong>(),
                        JsonValueKind.String => idNode.Deserialize<string?>() is { } idStr ? ulong.Parse(idStr) : null,
                        _ => throw new JsonException("Invalid emoji identifier")
                    };
                }

                if (obj.TryGetPropertyValue("name", out var nameNode) && nameNode is not null)
                {
                    name = nameNode.Deserialize<string?>();
                }

                if (obj.TryGetPropertyValue("animated", out var isAnimatedNode) && isAnimatedNode is not null)
                {
                    isAnimated = isAnimatedNode.Deserialize<bool>();
                }

                if (!id.HasValue && name is null)
                    throw new JsonException("Invalid emoji identifier");

                return new DiscordEmojiId(name, id, isAnimated);

            default:
                throw new JsonException("Invalid emoji identifier");
        }
    }

    public override void Write(Utf8JsonWriter writer, DiscordEmojiId value, JsonSerializerOptions options)
    {
        // written as a partial emoji object in the api

        if (value.Name is null && value.Id is null)
        {
            writer.WriteNullValue();
            return;
        }
        
        writer.WriteStartObject();
        
        if(value.Name is not null)
            writer.WriteString("name", value.Name);
        
        if(value.Id.HasValue)
            writer.WriteString("id", value.Id.Value.ToString());
        
        writer.WriteEndObject();
    }
}