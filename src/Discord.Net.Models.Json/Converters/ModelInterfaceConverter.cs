using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public class ModelInterfaceConverter<TModel, TUnderlying>(JsonTypeInfo<TUnderlying> typeInfo) : JsonConverter<TModel>
    where TModel : IModel
    where TUnderlying : IJsonModel, TModel, IApiModel<TModel, TUnderlying>
{
    public override TModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<TUnderlying>(ref reader, typeInfo);

    public override void Write(Utf8JsonWriter writer, TModel value, JsonSerializerOptions options)
    {
        if (value is not TUnderlying underlying)
            underlying = TUnderlying.From(value);
        
        JsonSerializer.Serialize(writer, underlying, typeInfo);
    }
}