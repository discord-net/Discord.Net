using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class VoiceChannelModelVariantConverter : JsonConverter<Discord.Models.IVoiceChannelModel>
{
    private readonly JsonTypeInfo<Discord.Models.Json.StageChannelModel> _stageChannelModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.VoiceChannelModel> _default;

    public VoiceChannelModelVariantConverter(
        JsonTypeInfo<Discord.Models.Json.StageChannelModel> stageChannelModel,
        JsonTypeInfo<Discord.Models.Json.VoiceChannelModel> @default
    )
    {
        _default = @default;
        _stageChannelModel = stageChannelModel;
    }
    
    public override Discord.Models.IVoiceChannelModel? Read(
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
            Discord.Models.ChannelType.GuildStageVoice => _stageChannelModel,
            _ => _default
        };
        
        return (Discord.Models.IVoiceChannelModel?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.IVoiceChannelModel value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.IStageChannelModel => _stageChannelModel,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}