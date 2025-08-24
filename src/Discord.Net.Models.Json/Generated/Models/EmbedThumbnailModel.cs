using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedThumbnailModel> EmbedThumbnailModel => field ??= Discord.Models.Json.EmbedThumbnailModel.CreateTypeInfo(Options);
}

public record EmbedThumbnailModel(
    string Url,
    Discord.Models.Optional<string> ProxyUrl,
    Discord.Models.Optional<int> Height,
    Discord.Models.Optional<int> Width
) : 
    IEmbedThumbnailModel,
    IJsonModel,
    IApiModel<IEmbedThumbnailModel, EmbedThumbnailModel>
{
    public static JsonTypeInfo<EmbedThumbnailModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedThumbnailModel>(
        options,
        new JsonObjectInfoValues<EmbedThumbnailModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedThumbnailModel(
                Url: (string)args[0],
                ProxyUrl: (Discord.Models.Optional<string>)args[1],
                Height: (Discord.Models.Optional<int>)args[2],
                Width: (Discord.Models.Optional<int>)args[3]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedThumbnailModel),
                Getter = static instance => ((Discord.Models.Json.EmbedThumbnailModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedThumbnailModel),
                Getter = static instance => ((Discord.Models.Json.EmbedThumbnailModel)instance).ProxyUrl,
                Setter = null,
                PropertyName = "ProxyUrl",
                JsonPropertyName = "proxy_url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedThumbnailModel),
                Getter = static instance => ((Discord.Models.Json.EmbedThumbnailModel)instance).Height,
                Setter = null,
                PropertyName = "Height",
                JsonPropertyName = "height",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedThumbnailModel),
                Getter = static instance => ((Discord.Models.Json.EmbedThumbnailModel)instance).Width,
                Setter = null,
                PropertyName = "Width",
                JsonPropertyName = "width",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Url",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ProxyUrl",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Height",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Width",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static EmbedThumbnailModel From(IEmbedThumbnailModel model) => (model as EmbedThumbnailModel) ?? new EmbedThumbnailModel(
        Url: model.Url,
        ProxyUrl: model.ProxyUrl,
        Height: model.Height,
        Width: model.Width
    );

    static EmbedThumbnailModel IApiModel<IEmbedThumbnailModel, EmbedThumbnailModel>.From(IEmbedThumbnailModel model) => From(model);
}