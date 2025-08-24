using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedProviderModel> EmbedProviderModel => field ??= Discord.Models.Json.EmbedProviderModel.CreateTypeInfo(Options);
}

public record EmbedProviderModel(
    Discord.Models.Optional<string> Name,
    Discord.Models.Optional<string> Url
) : 
    IEmbedProviderModel,
    IJsonModel,
    IApiModel<IEmbedProviderModel, EmbedProviderModel>
{
    public static JsonTypeInfo<EmbedProviderModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedProviderModel>(
        options,
        new JsonObjectInfoValues<EmbedProviderModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedProviderModel(
                Name: (Discord.Models.Optional<string>)args[0],
                Url: (Discord.Models.Optional<string>)args[1]
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
                DeclaringType = typeof(Discord.Models.Json.EmbedProviderModel),
                Getter = static instance => ((Discord.Models.Json.EmbedProviderModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedProviderModel),
                Getter = static instance => ((Discord.Models.Json.EmbedProviderModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Name",
           ParameterType = typeof(Discord.Models.Optional<string>),
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
        }
    ];

    public static EmbedProviderModel From(IEmbedProviderModel model) => (model as EmbedProviderModel) ?? new EmbedProviderModel(
        Name: model.Name,
        Url: model.Url
    );

    static EmbedProviderModel IApiModel<IEmbedProviderModel, EmbedProviderModel>.From(IEmbedProviderModel model) => From(model);
}