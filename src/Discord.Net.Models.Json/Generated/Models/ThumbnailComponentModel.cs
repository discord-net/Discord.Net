using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ThumbnailComponentModel> ThumbnailComponentModel => field ??= Discord.Models.Json.ThumbnailComponentModel.CreateTypeInfo(Options);
}

public record ThumbnailComponentModel(
    Discord.Models.IUnfurledMediaItemModel Media,
    Discord.Models.Optional<string> Description,
    Discord.Models.Optional<bool> Spoiler,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IThumbnailComponentModel,
    IJsonModel,
    IApiModel<IThumbnailComponentModel, ThumbnailComponentModel>
{
    public static JsonTypeInfo<ThumbnailComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ThumbnailComponentModel>(
        options,
        new JsonObjectInfoValues<ThumbnailComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ThumbnailComponentModel(
                Media: (Discord.Models.IUnfurledMediaItemModel)args[0],
                Description: (Discord.Models.Optional<string>)args[1],
                Spoiler: (Discord.Models.Optional<bool>)args[2],
                Type: (Discord.Models.ComponentType)args[3],
                Id: (Nullable<int>)args[4]
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
                DeclaringType = typeof(Discord.Models.Json.ThumbnailComponentModel),
                Getter = static instance => ((Discord.Models.Json.ThumbnailComponentModel)instance).Media,
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
                DeclaringType = typeof(Discord.Models.Json.ThumbnailComponentModel),
                Getter = static instance => ((Discord.Models.Json.ThumbnailComponentModel)instance).Description,
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
                DeclaringType = typeof(Discord.Models.Json.ThumbnailComponentModel),
                Getter = static instance => ((Discord.Models.Json.ThumbnailComponentModel)instance).Spoiler,
                Setter = null,
                PropertyName = "Spoiler",
                JsonPropertyName = "spoiler",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThumbnailComponentModel),
                Getter = static instance => ((Discord.Models.Json.ThumbnailComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ThumbnailComponentModel),
                Getter = static instance => ((Discord.Models.Json.ThumbnailComponentModel)instance).Id,
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
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static ThumbnailComponentModel From(IThumbnailComponentModel model) => (model as ThumbnailComponentModel) ?? new ThumbnailComponentModel(
        Media: model.Media,
        Description: model.Description,
        Spoiler: model.Spoiler,
        Type: model.Type,
        Id: model.Id
    );

    static ThumbnailComponentModel IApiModel<IThumbnailComponentModel, ThumbnailComponentModel>.From(IThumbnailComponentModel model) => From(model);
}