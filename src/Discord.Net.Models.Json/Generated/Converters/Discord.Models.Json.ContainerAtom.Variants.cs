using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class ContainerAtomVariantConverter : JsonConverter<Discord.Models.IContainerAtom>
{
    private readonly JsonTypeInfo<Discord.Models.Json.ActionRowComponentModel> _actionRowComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.FileComponentModel> _fileComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.MediaGalleryComponentModel> _mediaGalleryComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.SectionComponentModel> _sectionComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.SeparatorComponentItem> _separatorComponentItem;
    private readonly JsonTypeInfo<Discord.Models.Json.TextDisplayComponentModel> _textDisplayComponentModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.ContainerAtom> _default;

    public ContainerAtomVariantConverter(
        JsonTypeInfo<Discord.Models.Json.ActionRowComponentModel> actionRowComponentModel,
        JsonTypeInfo<Discord.Models.Json.FileComponentModel> fileComponentModel,
        JsonTypeInfo<Discord.Models.Json.MediaGalleryComponentModel> mediaGalleryComponentModel,
        JsonTypeInfo<Discord.Models.Json.SectionComponentModel> sectionComponentModel,
        JsonTypeInfo<Discord.Models.Json.SeparatorComponentItem> separatorComponentItem,
        JsonTypeInfo<Discord.Models.Json.TextDisplayComponentModel> textDisplayComponentModel,
        JsonTypeInfo<Discord.Models.Json.ContainerAtom> @default
    )
    {
        _default = @default;
        _actionRowComponentModel = actionRowComponentModel;
        _fileComponentModel = fileComponentModel;
        _mediaGalleryComponentModel = mediaGalleryComponentModel;
        _sectionComponentModel = sectionComponentModel;
        _separatorComponentItem = separatorComponentItem;
        _textDisplayComponentModel = textDisplayComponentModel;
    }
    
    public override Discord.Models.IContainerAtom? Read(
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
            Discord.Models.ComponentType.ActionRow => _actionRowComponentModel,
            Discord.Models.ComponentType.File => _fileComponentModel,
            Discord.Models.ComponentType.MediaGallery => _mediaGalleryComponentModel,
            Discord.Models.ComponentType.Section => _sectionComponentModel,
            Discord.Models.ComponentType.Separator => _separatorComponentItem,
            _ => _default
        };
        
        return (Discord.Models.IContainerAtom?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.IContainerAtom value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.ITextDisplayComponentModel => _textDisplayComponentModel,
            Discord.Models.IActionRowComponentModel => _actionRowComponentModel,
            Discord.Models.IFileComponentModel => _fileComponentModel,
            Discord.Models.IMediaGalleryComponentModel => _mediaGalleryComponentModel,
            Discord.Models.ISectionComponentModel => _sectionComponentModel,
            Discord.Models.ISeparatorComponentItem => _separatorComponentItem,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}