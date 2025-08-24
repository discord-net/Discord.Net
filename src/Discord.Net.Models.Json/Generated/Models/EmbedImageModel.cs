using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedImageModel> EmbedImageModel => field ??= Discord.Models.Json.EmbedImageModel.CreateTypeInfo(Options);
}

public record EmbedImageModel(
    string Url,
    Discord.Models.Optional<string> ProxyUrl,
    Discord.Models.Optional<int> Height,
    Discord.Models.Optional<int> Width
) : 
    IEmbedImageModel,
    IJsonModel,
    IApiModel<IEmbedImageModel, EmbedImageModel>
{
    public static JsonTypeInfo<EmbedImageModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedImageModel>(
        options,
        new JsonObjectInfoValues<EmbedImageModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedImageModel(
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
                DeclaringType = typeof(Discord.Models.Json.EmbedImageModel),
                Getter = static instance => ((Discord.Models.Json.EmbedImageModel)instance).Url,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedImageModel),
                Getter = static instance => ((Discord.Models.Json.EmbedImageModel)instance).ProxyUrl,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedImageModel),
                Getter = static instance => ((Discord.Models.Json.EmbedImageModel)instance).Height,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedImageModel),
                Getter = static instance => ((Discord.Models.Json.EmbedImageModel)instance).Width,
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

    public static EmbedImageModel From(IEmbedImageModel model) => (model as EmbedImageModel) ?? new EmbedImageModel(
        Url: model.Url,
        ProxyUrl: model.ProxyUrl,
        Height: model.Height,
        Width: model.Width
    );

    static EmbedImageModel IApiModel<IEmbedImageModel, EmbedImageModel>.From(IEmbedImageModel model) => From(model);
}