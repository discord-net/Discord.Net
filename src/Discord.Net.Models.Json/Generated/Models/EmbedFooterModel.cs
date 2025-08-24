using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedFooterModel> EmbedFooterModel => field ??= Discord.Models.Json.EmbedFooterModel.CreateTypeInfo(Options);
}

public record EmbedFooterModel(
    string Text,
    Discord.Models.Optional<string> IconUrl,
    Discord.Models.Optional<string> ProxyIconUrl
) : 
    IEmbedFooterModel,
    IJsonModel,
    IApiModel<IEmbedFooterModel, EmbedFooterModel>
{
    public static JsonTypeInfo<EmbedFooterModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedFooterModel>(
        options,
        new JsonObjectInfoValues<EmbedFooterModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedFooterModel(
                Text: (string)args[0],
                IconUrl: (Discord.Models.Optional<string>)args[1],
                ProxyIconUrl: (Discord.Models.Optional<string>)args[2]
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
                DeclaringType = typeof(Discord.Models.Json.EmbedFooterModel),
                Getter = static instance => ((Discord.Models.Json.EmbedFooterModel)instance).Text,
                Setter = null,
                PropertyName = "Text",
                JsonPropertyName = "text",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedFooterModel),
                Getter = static instance => ((Discord.Models.Json.EmbedFooterModel)instance).IconUrl,
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
                DeclaringType = typeof(Discord.Models.Json.EmbedFooterModel),
                Getter = static instance => ((Discord.Models.Json.EmbedFooterModel)instance).ProxyIconUrl,
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
           Name = "Text",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "IconUrl",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ProxyIconUrl",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static EmbedFooterModel From(IEmbedFooterModel model) => (model as EmbedFooterModel) ?? new EmbedFooterModel(
        Text: model.Text,
        IconUrl: model.IconUrl,
        ProxyIconUrl: model.ProxyIconUrl
    );

    static EmbedFooterModel IApiModel<IEmbedFooterModel, EmbedFooterModel>.From(IEmbedFooterModel model) => From(model);
}