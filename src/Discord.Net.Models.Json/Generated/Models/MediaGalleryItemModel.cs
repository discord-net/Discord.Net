using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MediaGalleryItemModel> MediaGalleryItemModel => field ??= Discord.Models.Json.MediaGalleryItemModel.CreateTypeInfo(Options);
}

public record MediaGalleryItemModel(
    Discord.Models.IUnfurledMediaItemModel Media,
    Discord.Models.Optional<string> Description,
    Discord.Models.Optional<bool> Spoiler
) : 
    IMediaGalleryItemModel,
    IJsonModel,
    IApiModel<IMediaGalleryItemModel, MediaGalleryItemModel>
{
    public static JsonTypeInfo<MediaGalleryItemModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MediaGalleryItemModel>(
        options,
        new JsonObjectInfoValues<MediaGalleryItemModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MediaGalleryItemModel(
                Media: (Discord.Models.IUnfurledMediaItemModel)args[0],
                Description: (Discord.Models.Optional<string>)args[1],
                Spoiler: (Discord.Models.Optional<bool>)args[2]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IUnfurledMediaItemModel>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IUnfurledMediaItemModel>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaGalleryItemModel),
                Getter = static instance => ((Discord.Models.Json.MediaGalleryItemModel)instance).Media,
                Setter = null,
                PropertyName = "Media",
                JsonPropertyName = "media",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaGalleryItemModel),
                Getter = static instance => ((Discord.Models.Json.MediaGalleryItemModel)instance).Description,
                Setter = null,
                PropertyName = "Description",
                JsonPropertyName = "description",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaGalleryItemModel),
                Getter = static instance => ((Discord.Models.Json.MediaGalleryItemModel)instance).Spoiler,
                Setter = null,
                PropertyName = "Spoiler",
                JsonPropertyName = "spoiler",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Media",
           ParameterType = typeof(Discord.Models.IUnfurledMediaItemModel),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Description",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Spoiler",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MediaGalleryItemModel From(IMediaGalleryItemModel model) => (model as MediaGalleryItemModel) ?? new MediaGalleryItemModel(
        Media: model.Media,
        Description: model.Description,
        Spoiler: model.Spoiler
    );

    static MediaGalleryItemModel IApiModel<IMediaGalleryItemModel, MediaGalleryItemModel>.From(IMediaGalleryItemModel model) => From(model);
}