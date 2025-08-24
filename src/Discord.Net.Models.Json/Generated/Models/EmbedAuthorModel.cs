using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedAuthorModel> EmbedAuthorModel => field ??= Discord.Models.Json.EmbedAuthorModel.CreateTypeInfo(Options);
}

public record EmbedAuthorModel(
    string Name,
    Discord.Models.Optional<string> Url,
    Discord.Models.Optional<string> IconUrl,
    Discord.Models.Optional<string> ProxyIconUrl
) : 
    IEmbedAuthorModel,
    IJsonModel,
    IApiModel<IEmbedAuthorModel, EmbedAuthorModel>
{
    public static JsonTypeInfo<EmbedAuthorModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedAuthorModel>(
        options,
        new JsonObjectInfoValues<EmbedAuthorModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedAuthorModel(
                Name: (string)args[0],
                Url: (Discord.Models.Optional<string>)args[1],
                IconUrl: (Discord.Models.Optional<string>)args[2],
                ProxyIconUrl: (Discord.Models.Optional<string>)args[3]
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
                DeclaringType = typeof(Discord.Models.Json.EmbedAuthorModel),
                Getter = static instance => ((Discord.Models.Json.EmbedAuthorModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedAuthorModel),
                Getter = static instance => ((Discord.Models.Json.EmbedAuthorModel)instance).Url,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedAuthorModel),
                Getter = static instance => ((Discord.Models.Json.EmbedAuthorModel)instance).IconUrl,
                Setter = null,
                PropertyName = "IconUrl",
                JsonPropertyName = "icon_url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedAuthorModel),
                Getter = static instance => ((Discord.Models.Json.EmbedAuthorModel)instance).ProxyIconUrl,
                Setter = null,
                PropertyName = "ProxyIconUrl",
                JsonPropertyName = "proxy_icon_url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Url",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "IconUrl",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ProxyIconUrl",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static EmbedAuthorModel From(IEmbedAuthorModel model) => (model as EmbedAuthorModel) ?? new EmbedAuthorModel(
        Name: model.Name,
        Url: model.Url,
        IconUrl: model.IconUrl,
        ProxyIconUrl: model.ProxyIconUrl
    );

    static EmbedAuthorModel IApiModel<IEmbedAuthorModel, EmbedAuthorModel>.From(IEmbedAuthorModel model) => From(model);
}