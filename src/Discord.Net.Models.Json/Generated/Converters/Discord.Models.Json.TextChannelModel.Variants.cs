using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class TextChannelModelVariantConverter : JsonConverter<Discord.Models.ITextChannelModel>
{
    private readonly JsonTypeInfo<Discord.Models.Json.AnnouncementChannelModel> _announcementChannelModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.TextChannelModel> _default;

    public TextChannelModelVariantConverter(
        JsonTypeInfo<Discord.Models.Json.AnnouncementChannelModel> announcementChannelModel,
        JsonTypeInfo<Discord.Models.Json.TextChannelModel> @default
    )
    {
        _default = @default;
        _announcementChannelModel = announcementChannelModel;
    }
    
    public override Discord.Models.ITextChannelModel? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (JsonNode.Parse(ref reader) is not JsonObject jsonObject)
            throw new JsonException("Expected object type");
            
        if (!jsonObject.TryGetPropertyValue("type", out var variant))
            return JsonSerializer.Deserialize(ref reader, _default);
            
        JsonTypeInfo info = variant.Deserialize<Discord.Models.ChannelType>(options) switch 
        {
            Discord.Models.ChannelType.GuildAnnouncement => _announcementChannelModel,
            _ => _default
        };
        
        return (Discord.Models.ITextChannelModel?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.ITextChannelModel value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.IAnnouncementChannelModel => _announcementChannelModel,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}