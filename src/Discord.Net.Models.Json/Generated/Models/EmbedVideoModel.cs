using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedVideoModel> EmbedVideoModel => field ??= Discord.Models.Json.EmbedVideoModel.CreateTypeInfo(Options);
}

public record EmbedVideoModel(
    Discord.Models.Optional<string> Url,
    Discord.Models.Optional<string> ProxyUrl,
    Discord.Models.Optional<int> Height,
    Discord.Models.Optional<int> Width
) : 
    IEmbedVideoModel,
    IJsonModel,
    IApiModel<IEmbedVideoModel, EmbedVideoModel>
{
    public static JsonTypeInfo<EmbedVideoModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedVideoModel>(
        options,
        new JsonObjectInfoValues<EmbedVideoModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedVideoModel(
                Url: (Discord.Models.Optional<string>)args[0],
                ProxyUrl: (Discord.Models.Optional<string>)args[1],
                Height: (Discord.Models.Optional<int>)args[2],
                Width: (Discord.Models.Optional<int>)args[3]
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
                DeclaringType = typeof(Discord.Models.Json.EmbedVideoModel),
                Getter = static instance => ((Discord.Models.Json.EmbedVideoModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedVideoModel),
                Getter = static instance => ((Discord.Models.Json.EmbedVideoModel)instance).ProxyUrl,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedVideoModel),
                Getter = static instance => ((Discord.Models.Json.EmbedVideoModel)instance).Height,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedVideoModel),
                Getter = static instance => ((Discord.Models.Json.EmbedVideoModel)instance).Width,
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
           ParameterType = typeof(Discord.Models.Optional<string>),
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

    public static EmbedVideoModel From(IEmbedVideoModel model) => (model as EmbedVideoModel) ?? new EmbedVideoModel(
        Url: model.Url,
        ProxyUrl: model.ProxyUrl,
        Height: model.Height,
        Width: model.Width
    );

    static EmbedVideoModel IApiModel<IEmbedVideoModel, EmbedVideoModel>.From(IEmbedVideoModel model) => From(model);
}