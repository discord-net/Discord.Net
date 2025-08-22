using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class IdOrModelConverter<TId, TModel> : JsonConverter<IdOrModel<TId, TModel>>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    private readonly JsonTypeInfo<TId> _idInfo;
    private readonly JsonTypeInfo _modelInfo;

    public IdOrModelConverter(JsonTypeInfo<TId> idInfo, JsonTypeInfo modelInfo)
    {
        _idInfo = idInfo;
        _modelInfo = modelInfo;
    }
    
    public override IdOrModel<TId, TModel> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.StartObject)
        {
            return (TModel)JsonSerializer.Deserialize(ref reader, _modelInfo)!;
        }
        
        return JsonSerializer.Deserialize<TId>(ref reader, _idInfo)!;
    }

    public override void Write(Utf8JsonWriter writer, IdOrModel<TId, TModel> value, JsonSerializerOptions options)
    {
        if (value.Model.IsSpecified)
        {
            JsonSerializer.Serialize(writer, value.Model, _modelInfo);
        }
        else
        {
            JsonSerializer.Serialize(writer, value.Id, _idInfo);
        }
    }
}