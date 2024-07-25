using Discord.Models;
using Discord.Models.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Converters;

public sealed class UserConverter : JsonConverter<User>
{
    private static JsonTypeInfo<User>? _userTypeInfo = null;

    private static JsonTypeInfo<User> GetNonConverterTypeInfo(JsonSerializerOptions options)
    {
        if (_userTypeInfo is not null)
            return _userTypeInfo;

        if (options.TypeInfoResolver is ModelJsonContext modelJsonContext)
            return _userTypeInfo = modelJsonContext.CreateUserTypeInfoNoConverter(options);

        return _userTypeInfo = options.TypeInfoResolverChain
            .OfType<ModelJsonContext>()
            .First()
            .CreateUserTypeInfoNoConverter(options);
    }

    public override User? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader).RootElement;

        if (doc.TryGetProperty("email", out _))
            return doc.Deserialize<SelfUser>(options);

        return doc.Deserialize(GetNonConverterTypeInfo(options));
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        if (value is SelfUser selfUser)
            JsonSerializer.Serialize(writer, selfUser, options);

        JsonSerializer.Serialize(writer, value, GetNonConverterTypeInfo(options));
    }
}
