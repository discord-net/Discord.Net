using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class SectionComponentAtomVariantConverter : JsonConverter<Discord.Models.ISectionComponentAtom>
{
    private readonly JsonTypeInfo<Discord.Models.Json.TextDisplayComponentModel> _textDisplayComponentModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.SectionComponentAtom> _default;

    public SectionComponentAtomVariantConverter(
        JsonTypeInfo<Discord.Models.Json.TextDisplayComponentModel> textDisplayComponentModel,
        JsonTypeInfo<Discord.Models.Json.SectionComponentAtom> @default
    )
    {
        _default = @default;
        _textDisplayComponentModel = textDisplayComponentModel;
    }
    
    public override Discord.Models.ISectionComponentAtom? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (JsonNode.Parse(ref reader) is not JsonObject jsonObject)
            throw new JsonException("Expected object type");
            
        if (!jsonObject.TryGetPropertyValue("type", out var variant))
            return JsonSerializer.Deserialize(ref reader, _default);
            
        JsonTypeInfo info = variant.Deserialize<Discord.Models.ComponentType>(options) switch 
        {
            Discord.Models.ComponentType.TextDisplay => _textDisplayComponentModel,
            _ => _default
        };
        
        return (Discord.Models.ISectionComponentAtom?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.ISectionComponentAtom value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.ITextDisplayComponentModel => _textDisplayComponentModel,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}