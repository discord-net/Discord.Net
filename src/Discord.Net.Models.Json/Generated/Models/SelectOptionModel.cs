using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<SelectOptionModel> SelectOptionModel => field ??= Discord.Models.Json.SelectOptionModel.CreateTypeInfo(Options);
}

public record SelectOptionModel(
    string Label,
    string Value,
    Discord.Models.Optional<string> Description,
    Discord.Models.Optional<Discord.Models.EmojiId> Emoji,
    Discord.Models.Optional<bool> Default
) : 
    ISelectOptionModel,
    IJsonModel,
    IApiModel<ISelectOptionModel, SelectOptionModel>
{
    public static JsonTypeInfo<SelectOptionModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<SelectOptionModel>(
        options,
        new JsonObjectInfoValues<SelectOptionModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new SelectOptionModel(
                Label: (string)args[0],
                Value: (string)args[1],
                Description: (Discord.Models.Optional<string>)args[2],
                Emoji: (Discord.Models.Optional<Discord.Models.EmojiId>)args[3],
                Default: (Discord.Models.Optional<bool>)args[4]
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
                DeclaringType = typeof(Discord.Models.Json.SelectOptionModel),
                Getter = static instance => ((Discord.Models.Json.SelectOptionModel)instance).Label,
                Setter = null,
                PropertyName = "Label",
                JsonPropertyName = "label",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SelectOptionModel),
                Getter = static instance => ((Discord.Models.Json.SelectOptionModel)instance).Value,
                Setter = null,
                PropertyName = "Value",
                JsonPropertyName = "value",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SelectOptionModel),
                Getter = static instance => ((Discord.Models.Json.SelectOptionModel)instance).Description,
                Setter = null,
                PropertyName = "Description",
                JsonPropertyName = "description",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.EmojiId>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.EmojiId>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SelectOptionModel),
                Getter = static instance => ((Discord.Models.Json.SelectOptionModel)instance).Emoji,
                Setter = null,
                PropertyName = "Emoji",
                JsonPropertyName = "emoji",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SelectOptionModel),
                Getter = static instance => ((Discord.Models.Json.SelectOptionModel)instance).Default,
                Setter = null,
                PropertyName = "Default",
                JsonPropertyName = "default",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Label",
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
           Name = "Description",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Emoji",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.EmojiId>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Default",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static SelectOptionModel From(ISelectOptionModel model) => (model as SelectOptionModel) ?? new SelectOptionModel(
        Label: model.Label,
        Value: model.Value,
        Description: model.Description,
        Emoji: model.Emoji,
        Default: model.Default
    );

    static SelectOptionModel IApiModel<ISelectOptionModel, SelectOptionModel>.From(ISelectOptionModel model) => From(model);
}