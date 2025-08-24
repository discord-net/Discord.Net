using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedModel> EmbedModel => field ??= Discord.Models.Json.EmbedModel.CreateTypeInfo(Options);
}

public record EmbedModel(
    Discord.Models.Optional<string> Title,
    Discord.Models.Optional<Discord.Models.EmbedType> Type,
    Discord.Models.Optional<string> Description,
    Discord.Models.Optional<string> Url,
    Discord.Models.Optional<DateTimeOffset> Timestamp,
    Discord.Models.Optional<Discord.Color> Color,
    Discord.Models.Optional<Discord.Models.IEmbedFooterModel> Footer,
    Discord.Models.Optional<Discord.Models.IEmbedImageModel> Image,
    Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel> Thumbnail,
    Discord.Models.Optional<Discord.Models.IEmbedVideoModel> Video,
    Discord.Models.Optional<Discord.Models.IEmbedProviderModel> Provider,
    Discord.Models.Optional<Discord.Models.IEmbedAuthorModel> Author,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>> Fields
) : 
    IEmbedModel,
    IJsonModel,
    IApiModel<IEmbedModel, EmbedModel>
{
    public static JsonTypeInfo<EmbedModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedModel>(
        options,
        new JsonObjectInfoValues<EmbedModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedModel(
                Title: (Discord.Models.Optional<string>)args[0],
                Type: (Discord.Models.Optional<Discord.Models.EmbedType>)args[1],
                Description: (Discord.Models.Optional<string>)args[2],
                Url: (Discord.Models.Optional<string>)args[3],
                Timestamp: (Discord.Models.Optional<DateTimeOffset>)args[4],
                Color: (Discord.Models.Optional<Discord.Color>)args[5],
                Footer: (Discord.Models.Optional<Discord.Models.IEmbedFooterModel>)args[6],
                Image: (Discord.Models.Optional<Discord.Models.IEmbedImageModel>)args[7],
                Thumbnail: (Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>)args[8],
                Video: (Discord.Models.Optional<Discord.Models.IEmbedVideoModel>)args[9],
                Provider: (Discord.Models.Optional<Discord.Models.IEmbedProviderModel>)args[10],
                Author: (Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>)args[11],
                Fields: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>)args[12]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Title,
                Setter = null,
                PropertyName = "Title",
                JsonPropertyName = "title",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.EmbedType>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.EmbedType>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Description,
                Setter = null,
                PropertyName = "Description",
                JsonPropertyName = "description",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<DateTimeOffset>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<DateTimeOffset>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Timestamp,
                Setter = null,
                PropertyName = "Timestamp",
                JsonPropertyName = "timestamp",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Color>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Color>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Color,
                Setter = null,
                PropertyName = "Color",
                JsonPropertyName = "color",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IEmbedFooterModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IEmbedFooterModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Footer,
                Setter = null,
                PropertyName = "Footer",
                JsonPropertyName = "footer",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IEmbedImageModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IEmbedImageModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Image,
                Setter = null,
                PropertyName = "Image",
                JsonPropertyName = "image",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Thumbnail,
                Setter = null,
                PropertyName = "Thumbnail",
                JsonPropertyName = "thumbnail",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IEmbedVideoModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IEmbedVideoModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Video,
                Setter = null,
                PropertyName = "Video",
                JsonPropertyName = "video",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IEmbedProviderModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IEmbedProviderModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Provider,
                Setter = null,
                PropertyName = "Provider",
                JsonPropertyName = "provider",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Author,
                Setter = null,
                PropertyName = "Author",
                JsonPropertyName = "author",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedModel),
                Getter = static instance => ((Discord.Models.Json.EmbedModel)instance).Fields,
                Setter = null,
                PropertyName = "Fields",
                JsonPropertyName = "fields",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Title",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.EmbedType>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Description",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Url",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Timestamp",
           ParameterType = typeof(Discord.Models.Optional<DateTimeOffset>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Color",
           ParameterType = typeof(Discord.Models.Optional<Discord.Color>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Footer",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IEmbedFooterModel>),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Image",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IEmbedImageModel>),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Thumbnail",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Video",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IEmbedVideoModel>),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Provider",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IEmbedProviderModel>),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Author",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Fields",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static EmbedModel From(IEmbedModel model) => (model as EmbedModel) ?? new EmbedModel(
        Title: model.Title,
        Type: model.Type,
        Description: model.Description,
        Url: model.Url,
        Timestamp: model.Timestamp,
        Color: model.Color,
        Footer: model.Footer,
        Image: model.Image,
        Thumbnail: model.Thumbnail,
        Video: model.Video,
        Provider: model.Provider,
        Author: model.Author,
        Fields: model.Fields
    );

    static EmbedModel IApiModel<IEmbedModel, EmbedModel>.From(IEmbedModel model) => From(model);
}