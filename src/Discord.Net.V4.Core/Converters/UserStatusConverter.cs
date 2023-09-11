using System.Text.Json.Serialization;
using System.Text.Json;

namespace Discord.Rest.Converters;

public class UserStatusConverter : JsonConverter<UserStatus>
{
    public static readonly UserStatusConverter Instance = new();

    public override UserStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeString = reader.GetString();

        return Enum.TryParse(typeString, true, out UserStatus value)
            ? value
            : UserStatus.Offline;
    }

    public override void Write(Utf8JsonWriter writer, UserStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLower());
    }
}
