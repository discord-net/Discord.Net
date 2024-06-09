using Discord.Models.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class EmoteConverter : JsonConverter<IEmote>
{
    public override IEmote? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader).RootElement;

        if (doc.TryGetProperty("id", out var snowflake) && snowflake.ValueKind is not JsonValueKind.Null)
            return doc.Deserialize<GuildEmote>(options);

        if (doc.TryGetProperty("name", out var name) && name.ValueKind is not JsonValueKind.Null)
            return doc.Deserialize<Emoji>();

        throw new JsonException("No emoji could be deserialized, missing id/name");
    }

    public override void Write(Utf8JsonWriter writer, IEmote value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
