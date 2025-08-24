using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MediaGalleryComponentModel> MediaGalleryComponentModel => field ??= Discord.Models.Json.MediaGalleryComponentModel.CreateTypeInfo(Options);
}

public record MediaGalleryComponentModel(
    System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel> Items,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IMediaGalleryComponentModel,
    IJsonModel,
    IApiModel<IMediaGalleryComponentModel, MediaGalleryComponentModel>
{
    public static JsonTypeInfo<MediaGalleryComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MediaGalleryComponentModel>(
        options,
        new JsonObjectInfoValues<MediaGalleryComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MediaGalleryComponentModel(
                Items: (System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>)args[0],
                Type: (Discord.Models.ComponentType)args[1],
                Id: (Nullable<int>)args[2]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaGalleryComponentModel),
                Getter = static instance => ((Discord.Models.Json.MediaGalleryComponentModel)instance).Items,
                Setter = null,
                PropertyName = "Items",
                JsonPropertyName = "items",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaGalleryComponentModel),
                Getter = static instance => ((Discord.Models.Json.MediaGalleryComponentModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<int>>(
            options,
            new JsonPropertyInfoValues<Nullable<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaGalleryComponentModel),
                Getter = static instance => ((Discord.Models.Json.MediaGalleryComponentModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Items",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static MediaGalleryComponentModel From(IMediaGalleryComponentModel model) => (model as MediaGalleryComponentModel) ?? new MediaGalleryComponentModel(
        Items: model.Items,
        Type: model.Type,
        Id: model.Id
    );

    static MediaGalleryComponentModel IApiModel<IMediaGalleryComponentModel, MediaGalleryComponentModel>.From(IMediaGalleryComponentModel model) => From(model);
}