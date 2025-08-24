using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<TextDisplayComponentModel> TextDisplayComponentModel => field ??= Discord.Models.Json.TextDisplayComponentModel.CreateTypeInfo(Options);
}

public record TextDisplayComponentModel(
    string Content,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    ITextDisplayComponentModel,
    IJsonModel,
    IApiModel<ITextDisplayComponentModel, TextDisplayComponentModel>
{
    public static JsonTypeInfo<TextDisplayComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<TextDisplayComponentModel>(
        options,
        new JsonObjectInfoValues<TextDisplayComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new TextDisplayComponentModel(
                Content: (string)args[0],
                Type: (Discord.Models.ComponentType)args[1],
                Id: (Nullable<int>)args[2]
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
                DeclaringType = typeof(Discord.Models.Json.TextDisplayComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextDisplayComponentModel)instance).Content,
                Setter = null,
                PropertyName = "Content",
                JsonPropertyName = "content",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextDisplayComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextDisplayComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.TextDisplayComponentModel),
                Getter = static instance => ((Discord.Models.Json.TextDisplayComponentModel)instance).Id,
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
           Name = "Content",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static TextDisplayComponentModel From(ITextDisplayComponentModel model) => (model as TextDisplayComponentModel) ?? new TextDisplayComponentModel(
        Content: model.Content,
        Type: model.Type,
        Id: model.Id
    );

    static TextDisplayComponentModel IApiModel<ITextDisplayComponentModel, TextDisplayComponentModel>.From(ITextDisplayComponentModel model) => From(model);
}