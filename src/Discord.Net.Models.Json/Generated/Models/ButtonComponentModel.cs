using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ButtonComponentModel> ButtonComponentModel => field ??= Discord.Models.Json.ButtonComponentModel.CreateTypeInfo(Options);
}

public record ButtonComponentModel(
    Discord.Models.ButtonStyle Style,
    Discord.Models.Optional<string> Label,
    Discord.Models.Optional<Discord.Models.EmojiId> Emoji,
    Discord.Models.Optional<Discord.Snowflake> SkuId,
    Discord.Models.Optional<string> Url,
    Discord.Models.Optional<bool> Disabled,
    string CustomId,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IButtonComponentModel,
    IJsonModel,
    IApiModel<IButtonComponentModel, ButtonComponentModel>
{
    public static JsonTypeInfo<ButtonComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ButtonComponentModel>(
        options,
        new JsonObjectInfoValues<ButtonComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ButtonComponentModel(
                Style: (Discord.Models.ButtonStyle)args[0],
                Label: (Discord.Models.Optional<string>)args[1],
                Emoji: (Discord.Models.Optional<Discord.Models.EmojiId>)args[2],
                SkuId: (Discord.Models.Optional<Discord.Snowflake>)args[3],
                Url: (Discord.Models.Optional<string>)args[4],
                Disabled: (Discord.Models.Optional<bool>)args[5],
                CustomId: (string)args[6],
                Type: (Discord.Models.ComponentType)args[7],
                Id: (Nullable<int>)args[8]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ButtonStyle>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ButtonStyle>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Style,
                Setter = null,
                PropertyName = "Style",
                JsonPropertyName = "style",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Label,
                Setter = null,
                PropertyName = "Label",
                JsonPropertyName = "label",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.EmojiId>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.EmojiId>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Emoji,
                Setter = null,
                PropertyName = "Emoji",
                JsonPropertyName = "emoji",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).SkuId,
                Setter = null,
                PropertyName = "SkuId",
                JsonPropertyName = "sku_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Disabled,
                Setter = null,
                PropertyName = "Disabled",
                JsonPropertyName = "disabled",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).CustomId,
                Setter = null,
                PropertyName = "CustomId",
                JsonPropertyName = "custom_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ButtonComponentModel),
                Getter = static instance => ((Discord.Models.Json.ButtonComponentModel)instance).Id,
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
           Name = "Style",
           ParameterType = typeof(Discord.Models.ButtonStyle),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Label",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Emoji",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.EmojiId>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "SkuId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Url",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Disabled",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "CustomId",
           ParameterType = typeof(string),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static ButtonComponentModel From(IButtonComponentModel model) => (model as ButtonComponentModel) ?? new ButtonComponentModel(
        Style: model.Style,
        Label: model.Label,
        Emoji: model.Emoji,
        SkuId: model.SkuId,
        Url: model.Url,
        Disabled: model.Disabled,
        CustomId: model.CustomId,
        Type: model.Type,
        Id: model.Id
    );

    static ButtonComponentModel IApiModel<IButtonComponentModel, ButtonComponentModel>.From(IButtonComponentModel model) => From(model);
}