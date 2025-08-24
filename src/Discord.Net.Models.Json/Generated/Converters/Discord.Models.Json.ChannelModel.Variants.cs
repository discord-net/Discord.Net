using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class ChannelModelVariantConverter : JsonConverter<Discord.Models.IChannelModel>
{
    private readonly JsonTypeInfo<Discord.Models.Json.AnnouncementChannelModel> _announcementChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.CategoryChannelModel> _categoryChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.DirectoryChannelModel> _directoryChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.DMChannelModel> _dMChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ForumChannelModel> _forumChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.GroupChannelModel> _groupChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.MediaChannelModel> _mediaChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.StageChannelModel> _stageChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.TextChannelModel> _textChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ThreadChannelModel> _threadChannelModel;
    private readonly JsonTypeInfo<Discord.Models.Json.VoiceChannelModel> _voiceChannelModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.ChannelModel> _default;

    public ChannelModelVariantConverter(
        JsonTypeInfo<Discord.Models.Json.AnnouncementChannelModel> announcementChannelModel,
        JsonTypeInfo<Discord.Models.Json.CategoryChannelModel> categoryChannelModel,
        JsonTypeInfo<Discord.Models.Json.DirectoryChannelModel> directoryChannelModel,
        JsonTypeInfo<Discord.Models.Json.DMChannelModel> dMChannelModel,
        JsonTypeInfo<Discord.Models.Json.ForumChannelModel> forumChannelModel,
        JsonTypeInfo<Discord.Models.Json.GroupChannelModel> groupChannelModel,
        JsonTypeInfo<Discord.Models.Json.MediaChannelModel> mediaChannelModel,
        JsonTypeInfo<Discord.Models.Json.StageChannelModel> stageChannelModel,
        JsonTypeInfo<Discord.Models.Json.TextChannelModel> textChannelModel,
        JsonTypeInfo<Discord.Models.Json.ThreadChannelModel> threadChannelModel,
        JsonTypeInfo<Discord.Models.Json.VoiceChannelModel> voiceChannelModel,
        JsonTypeInfo<Discord.Models.Json.ChannelModel> @default
    )
    {
        _default = @default;
        _announcementChannelModel = announcementChannelModel;
        _categoryChannelModel = categoryChannelModel;
        _directoryChannelModel = directoryChannelModel;
        _dMChannelModel = dMChannelModel;
        _forumChannelModel = forumChannelModel;
        _groupChannelModel = groupChannelModel;
        _mediaChannelModel = mediaChannelModel;
        _stageChannelModel = stageChannelModel;
        _textChannelModel = textChannelModel;
        _threadChannelModel = threadChannelModel;
        _voiceChannelModel = voiceChannelModel;
    }
    
    public override Discord.Models.IChannelModel? Read(
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
            Discord.Models.ChannelType.GuildStageVoice => _stageChannelModel,
            Discord.Models.ChannelType.GuildCategory => _categoryChannelModel,
            Discord.Models.ChannelType.GuildDirectory => _directoryChannelModel,
            Discord.Models.ChannelType.GuildForum => _forumChannelModel,
            Discord.Models.ChannelType.GuildMedia => _mediaChannelModel,
            Discord.Models.ChannelType.GuildText => _textChannelModel,
            Discord.Models.ChannelType.AnnouncementThread or Discord.Models.ChannelType.PublicThread or Discord.Models.ChannelType.PrivateThread => _threadChannelModel,
            Discord.Models.ChannelType.GuildVoice => _voiceChannelModel,
            Discord.Models.ChannelType.DM => _dMChannelModel,
            Discord.Models.ChannelType.GroupDM => _groupChannelModel,
            _ => _default
        };
        
        return (Discord.Models.IChannelModel?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.IChannelModel value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.IAnnouncementChannelModel => _announcementChannelModel,
            Discord.Models.IStageChannelModel => _stageChannelModel,
            Discord.Models.ICategoryChannelModel => _categoryChannelModel,
            Discord.Models.IDirectoryChannelModel => _directoryChannelModel,
            Discord.Models.IForumChannelModel => _forumChannelModel,
            Discord.Models.IMediaChannelModel => _mediaChannelModel,
            Discord.Models.ITextChannelModel => _textChannelModel,
            Discord.Models.IThreadChannelModel => _threadChannelModel,
            Discord.Models.IVoiceChannelModel => _voiceChannelModel,
            Discord.Models.IDMChannelModel => _dMChannelModel,
            Discord.Models.IGroupChannelModel => _groupChannelModel,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}