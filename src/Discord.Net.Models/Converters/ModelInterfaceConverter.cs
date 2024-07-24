using Discord.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public class ModelInterfaceConverter<TInterface, TModel> : JsonConverter<TInterface>
    where TInterface : class, IEntityModel
    where TModel : class, TInterface
{
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize(ref reader, typeof(TModel), options) as TInterface;
    }

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        if (value is not TModel model)
            throw new JsonException($"Expected {value.GetType()} ({typeof(TInterface)}) to be {typeof(TModel)}");

        JsonSerializer.Serialize(writer, model, options);
    }
}
