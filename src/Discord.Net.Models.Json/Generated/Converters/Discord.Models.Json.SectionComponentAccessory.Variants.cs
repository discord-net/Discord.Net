using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class SectionComponentAccessoryVariantConverter : JsonConverter<Discord.Models.ISectionComponentAccessory>
{
    private readonly JsonTypeInfo<Discord.Models.Json.ButtonComponentModel> _buttonComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ThumbnailComponentModel> _thumbnailComponentModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.SectionComponentAccessory> _default;

    public SectionComponentAccessoryVariantConverter(
        JsonTypeInfo<Discord.Models.Json.ButtonComponentModel> buttonComponentModel,
        JsonTypeInfo<Discord.Models.Json.ThumbnailComponentModel> thumbnailComponentModel,
        JsonTypeInfo<Discord.Models.Json.SectionComponentAccessory> @default
    )
    {
        _default = @default;
        _buttonComponentModel = buttonComponentModel;
        _thumbnailComponentModel = thumbnailComponentModel;
    }
    
    public override Discord.Models.ISectionComponentAccessory? Read(
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
            Discord.Models.ComponentType.Button => _buttonComponentModel,
            Discord.Models.ComponentType.Thumbnail => _thumbnailComponentModel,
            _ => _default
        };
        
        return (Discord.Models.ISectionComponentAccessory?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.ISectionComponentAccessory value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.IButtonComponentModel => _buttonComponentModel,
            Discord.Models.IThumbnailComponentModel => _thumbnailComponentModel,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}