using Discord.Models;
using Discord.Models.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class EmoteConverter : JsonConverter<IEmoteModel>
{
    public static readonly EmoteConverter Instance = new();

    public override IEmoteModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader).RootElement;

        if (doc.TryGetProperty("id", out var snowflake) && snowflake.ValueKind is not JsonValueKind.Null)
            return doc.Deserialize<CustomEmote>(options);

        if (doc.TryGetProperty("name", out var name) && name.ValueKind is not JsonValueKind.Null)
            return doc.Deserialize<Emoji>();

        throw new JsonException("No emoji could be deserialized, missing id/name");
    }

    public override void Write(Utf8JsonWriter writer, IEmoteModel value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
