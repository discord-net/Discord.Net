using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<EmbedFieldModel> EmbedFieldModel => field ??= Discord.Models.Json.EmbedFieldModel.CreateTypeInfo(Options);
}

public record EmbedFieldModel(
    string Name,
    string Value,
    Discord.Models.Optional<bool> Inline
) : 
    IEmbedFieldModel,
    IJsonModel,
    IApiModel<IEmbedFieldModel, EmbedFieldModel>
{
    public static JsonTypeInfo<EmbedFieldModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<EmbedFieldModel>(
        options,
        new JsonObjectInfoValues<EmbedFieldModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new EmbedFieldModel(
                Name: (string)args[0],
                Value: (string)args[1],
                Inline: (Discord.Models.Optional<bool>)args[2]
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
                DeclaringType = typeof(Discord.Models.Json.EmbedFieldModel),
                Getter = static instance => ((Discord.Models.Json.EmbedFieldModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedFieldModel),
                Getter = static instance => ((Discord.Models.Json.EmbedFieldModel)instance).Value,
                Setter = null,
                PropertyName = "Value",
                JsonPropertyName = "value",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.EmbedFieldModel),
                Getter = static instance => ((Discord.Models.Json.EmbedFieldModel)instance).Inline,
                Setter = null,
                PropertyName = "Inline",
                JsonPropertyName = "inline",
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
           Name = "Value",
           ParameterType = typeof(string),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Inline",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static EmbedFieldModel From(IEmbedFieldModel model) => (model as EmbedFieldModel) ?? new EmbedFieldModel(
        Name: model.Name,
        Value: model.Value,
        Inline: model.Inline
    );

    static EmbedFieldModel IApiModel<IEmbedFieldModel, EmbedFieldModel>.From(IEmbedFieldModel model) => From(model);
}