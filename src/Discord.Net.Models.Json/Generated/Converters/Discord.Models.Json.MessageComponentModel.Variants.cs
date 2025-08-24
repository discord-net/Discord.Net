using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class MessageComponentModelVariantConverter : JsonConverter<Discord.Models.IMessageComponentModel>
{
    private readonly JsonTypeInfo<Discord.Models.Json.ActionRowComponentModel> _actionRowComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ButtonComponentModel> _buttonComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ChannelSelectComponentModel> _channelSelectComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ContainerComponentModel> _containerComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.FileComponentModel> _fileComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.MediaGalleryComponentModel> _mediaGalleryComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.MentionableSelectComponentModel> _mentionableSelectComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.RoleSelectComponentModel> _roleSelectComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.SectionComponentModel> _sectionComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.SeparatorComponentItem> _separatorComponentItem;
    private readonly JsonTypeInfo<Discord.Models.Json.StringSelectComponentModel> _stringSelectComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.TextDisplayComponentModel> _textDisplayComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.TextInputComponentModel> _textInputComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.ThumbnailComponentModel> _thumbnailComponentModel;
    private readonly JsonTypeInfo<Discord.Models.Json.UserSelectComponentModel> _userSelectComponentModel;
    
    private readonly JsonTypeInfo<Discord.Models.Json.MessageComponentModel> _default;

    public MessageComponentModelVariantConverter(
        JsonTypeInfo<Discord.Models.Json.ActionRowComponentModel> actionRowComponentModel,
        JsonTypeInfo<Discord.Models.Json.ButtonComponentModel> buttonComponentModel,
        JsonTypeInfo<Discord.Models.Json.ChannelSelectComponentModel> channelSelectComponentModel,
        JsonTypeInfo<Discord.Models.Json.ContainerComponentModel> containerComponentModel,
        JsonTypeInfo<Discord.Models.Json.FileComponentModel> fileComponentModel,
        JsonTypeInfo<Discord.Models.Json.MediaGalleryComponentModel> mediaGalleryComponentModel,
        JsonTypeInfo<Discord.Models.Json.MentionableSelectComponentModel> mentionableSelectComponentModel,
        JsonTypeInfo<Discord.Models.Json.RoleSelectComponentModel> roleSelectComponentModel,
        JsonTypeInfo<Discord.Models.Json.SectionComponentModel> sectionComponentModel,
        JsonTypeInfo<Discord.Models.Json.SeparatorComponentItem> separatorComponentItem,
        JsonTypeInfo<Discord.Models.Json.StringSelectComponentModel> stringSelectComponentModel,
        JsonTypeInfo<Discord.Models.Json.TextDisplayComponentModel> textDisplayComponentModel,
        JsonTypeInfo<Discord.Models.Json.TextInputComponentModel> textInputComponentModel,
        JsonTypeInfo<Discord.Models.Json.ThumbnailComponentModel> thumbnailComponentModel,
        JsonTypeInfo<Discord.Models.Json.UserSelectComponentModel> userSelectComponentModel,
        JsonTypeInfo<Discord.Models.Json.MessageComponentModel> @default
    )
    {
        _default = @default;
        _actionRowComponentModel = actionRowComponentModel;
        _buttonComponentModel = buttonComponentModel;
        _channelSelectComponentModel = channelSelectComponentModel;
        _containerComponentModel = containerComponentModel;
        _fileComponentModel = fileComponentModel;
        _mediaGalleryComponentModel = mediaGalleryComponentModel;
        _mentionableSelectComponentModel = mentionableSelectComponentModel;
        _roleSelectComponentModel = roleSelectComponentModel;
        _sectionComponentModel = sectionComponentModel;
        _separatorComponentItem = separatorComponentItem;
        _stringSelectComponentModel = stringSelectComponentModel;
        _textDisplayComponentModel = textDisplayComponentModel;
        _textInputComponentModel = textInputComponentModel;
        _thumbnailComponentModel = thumbnailComponentModel;
        _userSelectComponentModel = userSelectComponentModel;
    }
    
    public override Discord.Models.IMessageComponentModel? Read(
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
            Discord.Models.ComponentType.Button => _buttonComponentModel,
            Discord.Models.ComponentType.File => _fileComponentModel,
            Discord.Models.ComponentType.MediaGallery => _mediaGalleryComponentModel,
            Discord.Models.ComponentType.Section => _sectionComponentModel,
            Discord.Models.ComponentType.Separator => _separatorComponentItem,
            Discord.Models.ComponentType.Thumbnail => _thumbnailComponentModel,
            Discord.Models.ComponentType.ChannelSelect => _channelSelectComponentModel,
            Discord.Models.ComponentType.Container => _containerComponentModel,
            Discord.Models.ComponentType.MentionableSelect => _mentionableSelectComponentModel,
            Discord.Models.ComponentType.RoleSelect => _roleSelectComponentModel,
            Discord.Models.ComponentType.StringSelect => _stringSelectComponentModel,
            Discord.Models.ComponentType.TextInput => _textInputComponentModel,
            Discord.Models.ComponentType.UserSelect => _userSelectComponentModel,
            _ => _default
        };
        
        return (Discord.Models.IMessageComponentModel?)jsonObject.Deserialize(info);
    }
    
    public override void Write(Utf8JsonWriter writer, Discord.Models.IMessageComponentModel value, JsonSerializerOptions options) 
    {
        JsonTypeInfo info = value switch 
        {
            Discord.Models.ITextDisplayComponentModel => _textDisplayComponentModel,
            Discord.Models.IActionRowComponentModel => _actionRowComponentModel,
            Discord.Models.IButtonComponentModel => _buttonComponentModel,
            Discord.Models.IFileComponentModel => _fileComponentModel,
            Discord.Models.IMediaGalleryComponentModel => _mediaGalleryComponentModel,
            Discord.Models.ISectionComponentModel => _sectionComponentModel,
            Discord.Models.ISeparatorComponentItem => _separatorComponentItem,
            Discord.Models.IThumbnailComponentModel => _thumbnailComponentModel,
            Discord.Models.IChannelSelectComponentModel => _channelSelectComponentModel,
            Discord.Models.IContainerComponentModel => _containerComponentModel,
            Discord.Models.IMentionableSelectComponentModel => _mentionableSelectComponentModel,
            Discord.Models.IRoleSelectComponentModel => _roleSelectComponentModel,
            Discord.Models.IStringSelectComponentModel => _stringSelectComponentModel,
            Discord.Models.ITextInputComponentModel => _textInputComponentModel,
            Discord.Models.IUserSelectComponentModel => _userSelectComponentModel,
            _ => _default
        };
        
        JsonSerializer.Serialize(writer, value, info);
    }
}