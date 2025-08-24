using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<TextInputComponentModel> TextInputComponentModel => field ??= Discord.Models.Json.TextInputComponentModel.CreateTypeInfo(Options);
}

public record TextInputComponentModel(
    Discord.Models.TextInputStyle Style,
    string Label,
    Discord.Models.Optional<int> MinLength,
    Discord.Models.Optional<int> MaxLength,
    Discord.Models.Optional<bool> Required,
    Discord.Models.Optional<string> Value,
    Discord.Models.Optional<string> Placeholder,
    Discord.Models.ComponentType Type,
    Nullable<int> Id,
    string CustomId
) : 
    ITextInputComponentModel,
    IJsonModel,
    IApiModel<ITextInputComponentModel, TextInputComponentModel>
{
    public static JsonTypeInfo<TextInputComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<TextInputComponentModel>(
        options,
        new JsonObjectInfoValues<TextInputComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new TextInputComponentModel(
                Style: (Discord.Models.TextInputStyle)args[0],
                Label: (string)args[1],
                MinLength: (Discord.Models.Optional<int>)args[2],
                MaxLength: (Discord.Models.Optional<int>)args[3],
                Required: (Discord.Models.Optional<bool>)args[4],
                Value: (Discord.Models.Optional<string>)args[5],
                Placeholder: (Discord.Models.Optional<string>)args[6],
                Type: (Discord.Models.ComponentType)args[7],
                Id: (Nullable<int>)args[8],
                CustomId: (string)args[9]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.TextInputStyle>(
            options,
            new JsonPropertyInfoValues<Discord.Models.TextInputStyle>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Style,
                Setter = null,
                PropertyName = "Style",
                JsonPropertyName = "style",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Label,
                Setter = null,
                PropertyName = "Label",
                JsonPropertyName = "label",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).MinLength,
                Setter = null,
                PropertyName = "MinLength",
                JsonPropertyName = "min_length",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).MaxLength,
                Setter = null,
                PropertyName = "MaxLength",
                JsonPropertyName = "max_length",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Required,
                Setter = null,
                PropertyName = "Required",
                JsonPropertyName = "required",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Value,
                Setter = null,
                PropertyName = "Value",
                JsonPropertyName = "value",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Placeholder,
                Setter = null,
                PropertyName = "Placeholder",
                JsonPropertyName = "placeholder",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextInputComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextInputComponentModel)instance).CustomId,
                Setter = null,
                PropertyName = "CustomId",
                JsonPropertyName = "custom_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Style",
           ParameterType = typeof(Discord.Models.TextInputStyle),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Label",
           ParameterType = typeof(string),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MinLength",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MaxLength",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Required",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Value",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Placeholder",
           ParameterType = typeof(Discord.Models.Optional<string>),
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
        },
        new()
        {
           Name = "CustomId",
           ParameterType = typeof(string),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static TextInputComponentModel From(ITextInputComponentModel model) => (model as TextInputComponentModel) ?? new TextInputComponentModel(
        Style: model.Style,
        Label: model.Label,
        MinLength: model.MinLength,
        MaxLength: model.MaxLength,
        Required: model.Required,
        Value: model.Value,
        Placeholder: model.Placeholder,
        Type: model.Type,
        Id: model.Id,
        CustomId: model.CustomId
    );

    static TextInputComponentModel IApiModel<ITextInputComponentModel, TextInputComponentModel>.From(ITextInputComponentModel model) => From(model);
}